using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour {

    public static bool mouseReleased = false;

    public Toggle Toggle1;
    public Toggle Toggle2;
    public Toggle Toggle3;

    [SerializeField] bool active = true;
    [SerializeField] bool cutsceneMode = false;
    GameObject player;
    Transform target;

    [Header("Camera Coordinates")]

    [SerializeField] float minX; //Booundaries of level
    [SerializeField] float maxX;
    [SerializeField] float minY;
    [SerializeField] float maxY;
    [SerializeField] Vector3 offset = new Vector3(0, 1, -3); // TODO: Remove?
    [SerializeField] Vector3 defaultRotation;

    [Header ("Rotation")]

    [SerializeField] bool followPlayer;
    [SerializeField] bool followWhileMoving;
    [SerializeField] bool followIgnoreBackwardsMovement;
    [SerializeField] bool followTurnAfterBackwardsMovement;
    [SerializeField] bool invertYAxis;
    [SerializeField] float cameraMouseSensitivity = 1f;
    [SerializeField] float freeLookDetectionSensitivity = 0.05f;
    [SerializeField] float verticalRotationMax;
    [SerializeField] float verticalRotationMin;
    [SerializeField] float xRotation = 0.0f;
    [SerializeField] float yRotation = 0.0f;
    [SerializeField] Quaternion rotation;
    [SerializeField] Vector3 rotationDelta;

    [Header ("Screen Shake")]

    [SerializeField] float shakeFrequency;
    [SerializeField] float shakeAmplitude;
    [Range (0, 1)]
    [SerializeField] float shakeAmount;
    [SerializeField] float shakeDamp;
    [SerializeField] bool dampShake = true;

    [Header ("Zoom")]

    [SerializeField] float zoom = -3f;
    [SerializeField] float zoomOffsetMax = 7f;
    [SerializeField] float zoomOffsetMin = 1.5f;
    [SerializeField] float zoomAmount = 0.2f;

    [Header ("Aim")]

    [SerializeField] float aimCameraSensitivityMultiplier = 0.5f;
    Transform leftShoulder;
    Transform rightShoulder;
    bool targetingRightShoulder = true;

    [Header ("Smoothing")]

    [SerializeField] float aimBlendSpeed = 0.5f;
    [SerializeField] float freeLookReturnDelay = 2;
    [SerializeField] float freeLookReturnBlendSpeed = 8;
    float freeLookReturnIterator;
    float freeLookBlend = 1;
    [SerializeField] float movementInterpolationSpeed = 0.05f;
    [SerializeField] float aimingMovementInterpolationSpeed = 0.05f;
    [SerializeField] float rotationInterpolationFactor = 0.2f;
    [SerializeField] float followRotationInterpolationSpeed = 0.5f;
    bool isFreeLooking;
    bool isFreeCam;
    bool wasAiming;
    float aimingBlend = 0;

    [Header ("Collision")]

    [SerializeField] float detectionRadius = 1.5f;
    [SerializeField] float shoulderDetectionRadius = 0.5f;
    [SerializeField] float cameraDistanceBuffer = 0.2f;
    [SerializeField] LayerMask collisionMask;

    PlayerControlMapping control;
    Animator anim;
    SceneData sData;
    Camera cam;
    RaycastHit ray;

    Vector3 standardPosition;
    Vector3 shoulderPosition;
    Vector3 targetPosition;
    Quaternion standardRotation;
    Quaternion shoulderRotation;
    Quaternion targetRotation;
    float freeLookCacheXRotation;
    float freeLookCacheYRotation;
    float distanceCache;
    bool playerMovingBackward = false;

    float cutsceneTargetInterpolationSpeed;
    float cutsceneTargetSetTime;
    Vector3 cutsceneCachePosition;
    Quaternion cutsceneCacheRotation;

    private void Awake () {
        player = GameObject.FindGameObjectWithTag ("Player");
        target = player.transform.Find ("CameraTargetC");
        leftShoulder = player.transform.Find ("CameraTargetL");
        rightShoulder = player.transform.Find ("CameraTargetR");
        cam = GetComponent<Camera> ();
        anim = transform.parent.GetComponent<Animator> ();
        control = player.GetComponent<PlayerControlMapping> ();
        sData = FindObjectOfType<SceneData> ();// GameObject.FindGameObjectWithTag("Canvas").GetComponent<SceneData>();
    }

    private void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        freeLookReturnIterator = freeLookReturnDelay;

        ResetCamera ();
    }

    private void Update () {
        if (!active || cutsceneMode) { return; }

        if (control.swapShoulders) {
            targetingRightShoulder = !targetingRightShoulder;
        }

        if (control.freeMouse) {
            mouseReleased = !mouseReleased;
        }
    }

    // Update is called once per frame
    void FixedUpdate() {

        if (!active) { return; }

        if (cutsceneMode) {
            /*transform.position = Vector3.Lerp (cutsceneCachePosition, targetPosition, Mathf.Clamp01 ((Time.time - cutsceneTargetSetTime) * cutsceneTargetInterpolationSpeed));
            transform.rotation = Quaternion.Lerp (cutsceneCacheRotation, targetRotation, Mathf.Clamp01 ((Time.time - cutsceneTargetSetTime) * cutsceneTargetInterpolationSpeed));*/
            return;
        }

        // Input
        GetInput();
        Zoom ();
        //print (yRotation);

        // Calculate target position and rotation
        TargetPosition ();
        TargetRotation ();
        // TODO: Smooth movement and rotation while returning to default camera behavior from freelook behavior

        // Actually move and rotate the camera
        if (!active) { return; } // Camera updates its target transform during cutscenes so it can cut to them when they finish
        ApplyTransform ();
        ShakeScreen (); // TODO: allow screenshake in cutscenes, or fake it
    }

    void GetInput () {
        // Takes a threshold of movement to enable freelook because otherwise freelook overrides the camera contstantly if the player rests their hand on it
        isFreeLooking = mouseReleased ? false : (isFreeLooking ? (Mathf.Abs (control.horizontalAim) > Mathf.Epsilon || Mathf.Abs (control.verticalAim) > Mathf.Epsilon)
                                                               : (Mathf.Abs (control.horizontalAim) > freeLookDetectionSensitivity || Mathf.Abs (control.verticalAim) > freeLookDetectionSensitivity));
        if (followTurnAfterBackwardsMovement || Mathf.Abs (control.vMove) > Mathf.Epsilon) {
            playerMovingBackward = control.vMove < -Mathf.Epsilon;
        }

        // Ignore mouse input if player isn't aiming or freelooking
        if (control.aiming || isFreeLooking) {
            rotationDelta.y = control.horizontalAim * cameraMouseSensitivity;
            rotationDelta.x = (invertYAxis ? -1 : 1) * control.verticalAim * cameraMouseSensitivity;
        } else {
            rotationDelta = Vector3.zero;
        }

        // Get mouse rotation if free-looking; otherwise, use player rotation
        if (!control.aiming) {
            if (isFreeCam || (!followWhileMoving && (Mathf.Abs (control.vMove) > Mathf.Epsilon || Mathf.Abs (control.xMove) > Mathf.Epsilon))) {
                yRotation += rotationDelta.y; //Capture horizontal mouse movement
                xRotation += rotationDelta.x; //Capture vertical mouse movement
                xRotation = Mathf.Clamp (xRotation, verticalRotationMin, verticalRotationMax); //Clamp vertical movement to certain angles
                freeLookCacheXRotation = xRotation;
                freeLookCacheYRotation = yRotation;
            } else {
                xRotation = Mathf.LerpAngle (defaultRotation.x, freeLookCacheXRotation, freeLookBlend);
                yRotation = Mathf.LerpAngle (Mathf.LerpAngle (yRotation, target.rotation.eulerAngles.y + (playerMovingBackward && followIgnoreBackwardsMovement ? 180 : 0), followRotationInterpolationSpeed * Time.deltaTime), freeLookCacheYRotation, freeLookBlend);
            }

            rotation = Quaternion.Euler (-xRotation, yRotation, 0);
        }

        // Caching for effects
        isFreeCam = followPlayer ? freeLookReturnIterator < freeLookReturnDelay : true;
        wasAiming = control.aiming;

        // Return freelook to normal with blending after a delay. Shortens delay if player moves, and skips it entirely if player aims
        if (isFreeLooking) {
            freeLookReturnIterator = 0;
            freeLookBlend = 1;
        } else if (control.aiming || ((Mathf.Abs (control.xMove) > Mathf.Epsilon || Mathf.Abs (control.vMove) > Mathf.Epsilon) && freeLookReturnIterator > freeLookReturnDelay / 4)) {
            freeLookReturnIterator = freeLookReturnDelay;
        } else if (!mouseReleased) {
            freeLookReturnIterator += Time.deltaTime;
            freeLookReturnIterator = Mathf.Clamp (freeLookReturnIterator, 0, freeLookReturnDelay);
        }
        if (freeLookReturnIterator == freeLookReturnDelay) {
            freeLookBlend -= Time.deltaTime * freeLookReturnBlendSpeed;
            freeLookBlend = Mathf.Clamp01 (freeLookBlend);
        }

        // Blends into and out of aiming
        aimingBlend += Time.deltaTime * aimBlendSpeed * (control.aiming ? 1f : -1f);
        aimingBlend = Mathf.Clamp01 (aimingBlend);
    }

    void Zoom () {
        if (control.scroll != 0) //If player is scrolling
        {
            zoom += control.scroll * zoomAmount; //Scroll is +1 or -1
        }

        zoom = Mathf.Clamp (zoom, -zoomOffsetMax, -zoomOffsetMin); //Clamp the zoom allowed
    }

    void TargetPosition () {
        TargetShoulderPositions ();
        TargetStandardPosition ();

        targetPosition = Vector3.Lerp (standardPosition, shoulderPosition, aimingBlend);
    }

    void TargetShoulderPositions () {

        Vector3 normPos = targetingRightShoulder ? rightShoulder.position : leftShoulder.position;
        //Debug.DrawRay(target.transform.position, normPos - target.transform.position, Color.red, 2);

        // SphereCasts back a bit from the camera, making sure that the camera does not get clip into a wall if the player backs up
        if (Physics.CheckSphere (normPos, shoulderDetectionRadius, collisionMask)) {
            // Repositions the camera in front of the obstacle
            // Places the camera at the distance of the rayuast impact along the original line,
            // to allow us to use a spherecast while keeping the camera from snapping to the edge of surfaces
            targetingRightShoulder = !targetingRightShoulder;
            normPos = targetingRightShoulder ? rightShoulder.position : leftShoulder.position;
            shoulderPosition = normPos;

            //shoulderPosition = normPos - target.transform.forward * (ray.point - normPos).magnitude * (1 - cameraDistanceBuffer);
            //Debug.Log("hit");
        } else {
            shoulderPosition = normPos; //In case no obstruction, use normal position
        }
    }

    void TargetStandardPosition () {

        Vector3 normPos = target.transform.position + rotation * offset.normalized * -zoom; // An unobstructed camera's position
        //Debug.DrawRay(target.transform.position, normPos - target.transform.position, Color.red, 2);

        // SphereCasts from the vision target back to the camera, to ensure that the first target hit is always the foremost
        if(Physics.SphereCast (target.transform.position, detectionRadius, normPos - target.transform.position, out ray, -zoom, collisionMask)) {
            // Repositions the camera in front of the obstacle
            // Places the camera at the distance of the rayuast impact along the original line,
            // to allow us to use a spherecast while keeping the camera from snapping to the edge of surfaces
            standardPosition = (normPos - target.transform.position).normalized *
                             (ray.point - target.transform.position).magnitude *
                             (1 - cameraDistanceBuffer) + target.transform.position;
            //Debug.Log("hit");
            // When an obstacle is hit, snap to the hit position
            distanceCache = (ray.point - target.transform.position).magnitude;
        }
        else
        {
            standardPosition = normPos; //In case no obstruction, use normal position
            // When an obstacle was hit previously but is not hit now, slowly zoom out instead of snapping out
            distanceCache = Mathf.Lerp (distanceCache, (normPos - target.transform.position).magnitude, (1 - Mathf.Clamp01 (rotationDelta.magnitude)) * Time.fixedDeltaTime);
        }
    }

    void TargetRotation () {
        TargetStandardRotations ();
        TargetShoulderRotations ();

        targetRotation = Quaternion.Lerp (standardRotation, shoulderRotation, aimingBlend);
    }

    void TargetStandardRotations () {
        standardRotation = Quaternion.LookRotation (target.transform.position - transform.position);
        //standardRotation = Quaternion.Lerp (standardRotation, standardTargetRotation, followRotationInterpolationSpeed * Time.deltaTime);
    }

    void TargetShoulderRotations () {
        shoulderRotation = targetingRightShoulder ? rightShoulder.rotation : leftShoulder.rotation;
        shoulderRotation = Quaternion.Lerp (shoulderRotation, Quaternion.LookRotation (cam.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 50)) - transform.position), aimCameraSensitivityMultiplier);
    }

    void ApplyTransform () {
        if (control.aiming) {
            transform.position = Vector3.Lerp (transform.position, targetPosition, aimingMovementInterpolationSpeed);
        } else {
            transform.position = Vector3.Lerp (transform.position, targetPosition, movementInterpolationSpeed);
        }
        transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, rotationInterpolationFactor);
    }

    void ShakeScreen () {

        // Normalized noise vector with x and y values ranging from -1 to 1
        Vector3 shakeOffset = new Vector3 (Mathf.PerlinNoise (Time.time * shakeFrequency, 0) * 2 - 1, Mathf.PerlinNoise (0, Time.time * shakeFrequency) * 2 - 1, 0).normalized;
        // Randomizes amplitude then scales it by shakeAmplitude and shakeAmount
        shakeOffset *= Mathf.PerlinNoise (Time.time * shakeFrequency / Statics.Sqrt2, Time.time * shakeFrequency / Statics.Sqrt2) * shakeAmplitude * shakeAmount;
        // Translates the camera along its local x and y axes. Ignores z axis
        transform.position += transform.TransformVector (shakeOffset);

        //Debug.DrawRay (transform.position, shakeOffset * 5, Color.red, 2);
        //Debug.DrawRay (transform.TransformVector (transform.position), shakeOffset * 5, Color.blue, 2);

        if (!dampShake) { return; }

        // Sets shakeAmount to 0 when it gets low to make sure that screen shake completely stops
        if (shakeAmount > 0) {
            shakeAmount -= Time.deltaTime * shakeDamp;
        } else {
            shakeAmount = 0;
        }
    }

    public void AddScreenShake (float shakeIncrease) {
        shakeAmount = Mathf.Clamp01 (shakeAmount + shakeIncrease);
    }

    private void OnEnable () {
        CutsceneManager.CutsceneStart += StartCutscene;
        CutsceneManager.CutsceneStop += StopCutscene;
    }

    private void OnDisable () {
        CutsceneManager.CutsceneStart -= StartCutscene;
        CutsceneManager.CutsceneStop -= StopCutscene;
    }

    void ResetCamera () {
        xRotation = defaultRotation.x;
        yRotation = target.rotation.eulerAngles.y;
        rotation = Quaternion.Euler (-xRotation, yRotation, 0);

        targetPosition = target.transform.position + rotation * offset.normalized * -zoom;
        distanceCache = (targetPosition - target.transform.position).magnitude;
        targetRotation = Quaternion.LookRotation (target.transform.position - transform.position);
        transform.position = targetPosition;
        transform.rotation = targetRotation;

        freeLookReturnIterator = 0;
        freeLookBlend = 0;
        aimingBlend = 0;
        wasAiming = false;
        isFreeLooking = false;
    }

    public void SetYAxisInvert (bool input) {
        invertYAxis = input;
    }

    public void SetPlayerFollow (bool input) {
        followPlayer = input;
        Toggle1.interactable = input;
        Toggle3.interactable = input;
    }

    public void SetFollowWhileMoving (bool input) {
        followWhileMoving = input;
    }

    public void SetFollowIgnoreBackwardsMovement (bool input) {
        followIgnoreBackwardsMovement = !input;
        Toggle2.interactable = !input;
    }

    public void SetFollowTurnAfterBackwardsMovement (bool input) {
        followTurnAfterBackwardsMovement = input;
    }

    public void SetSensitivity (float input) {
        cameraMouseSensitivity = Mathf.Pow (input, 2);
    }

    // Cutscene methods

    public void StartCutscene () {
        cutsceneMode = true;
        anim.enabled = true;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    public void StopCutscene () {
        targetPosition = transform.parent.position;
        targetRotation = transform.parent.rotation;
        cutsceneMode = false;
        anim.enabled = false;
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        transform.parent.position = Vector3.zero;
        transform.parent.rotation = Quaternion.identity;
        ResetCamera ();
    }
}
