using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour
{
    [SerializeField] bool active = true;
    GameObject player;
    Transform target;

    [Header("Camera Coordinates")]

    [SerializeField] float minX; //Booundaries of level
    [SerializeField] float maxX;
    [SerializeField] float minY;
    [SerializeField] float maxY;
    [SerializeField] Vector3 offset = new Vector3(0, 1, -3);
    [SerializeField] Vector3 defaultRotation;
    float offsetMagnitude;

    [Header ("Rotation")]

    [SerializeField] bool invertYAxis;
    [SerializeField] float cameraSensitivity = 3f;
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
    [SerializeField] float rotationInterpolationFactor = 0.2f;
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
    bool mouseReleased = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag ("Player");
        target = player.transform.Find ("CameraTargetC");
        leftShoulder = player.transform.Find ("CameraTargetL");
        rightShoulder = player.transform.Find ("CameraTargetR");
        cam = GetComponent<Camera> ();
        control = player.GetComponent<PlayerControlMapping>();
        sData = FindObjectOfType<SceneData> ();// GameObject.FindGameObjectWithTag("Canvas").GetComponent<SceneData>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.position = offset;
        xRotation = defaultRotation.x;
        yRotation = defaultRotation.y;
        freeLookReturnIterator = freeLookReturnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        // Input
        GetInput();
        Zoom ();

        // Calculate target position and rotation
        TargetPosition ();
        TargetRotation ();
        // TODO: Smooth movement and rotation while returning to default camera behavior from freelook behavior

        // Actually move and rotate the camera
        if (!active) { return; } // Camera updates its target transform during cutscenes so it can cut to them when they finish
        ApplyTransform ();
        UpdateCursor ();
        ShakeScreen (); // TODO: allow screenshake in cutscenes, or fake it
    }

    void GetInput () {
        isFreeLooking = Mathf.Abs (control.horizontalAim) > Mathf.Epsilon || Mathf.Abs (control.verticalAim) > Mathf.Epsilon;

        // Ignore mouse input if player isn't aiming or freelooking
        if (control.aiming || (isFreeLooking && !mouseReleased)) {
            rotationDelta.y = control.horizontalAim * cameraSensitivity;
            rotationDelta.x = (invertYAxis ? -1 : 1) * control.verticalAim * cameraSensitivity;
        } else {
            rotationDelta = Vector3.zero;
        }

        // Get mouse rotation if free-looking; otherwise, use player rotation
        if (!control.aiming) {
            if (isFreeLooking || freeLookReturnIterator < freeLookReturnDelay) {
                yRotation += rotationDelta.y; //Capture horizontal mouse movement
                xRotation += rotationDelta.x; //Capture vertical mouse movement
                xRotation = Mathf.Clamp (xRotation, verticalRotationMin, verticalRotationMax); //Clamp vertical movement to certain angles
                freeLookCacheXRotation = xRotation;
                freeLookCacheYRotation = yRotation;
            } else {
                xRotation = Mathf.Lerp (defaultRotation.x, freeLookCacheXRotation, freeLookBlend);
                yRotation = Mathf.Lerp (player.transform.rotation.eulerAngles.y, freeLookCacheYRotation, freeLookBlend);
            }

            rotation = Quaternion.Euler (-xRotation, yRotation, 0);
        }

        // Caching for effects
        isFreeCam = freeLookReturnIterator < freeLookReturnDelay;
        wasAiming = control.aiming;

        // Return freelook to normal with blending after a delay. Shortens delay if player moves, and skips it entirely if player aims
        if (isFreeLooking) {
            freeLookReturnIterator = 0;
            freeLookBlend = 1;
        } else if (control.aiming || ((Mathf.Abs (control.xMove) > Mathf.Epsilon || Mathf.Abs (control.vMove) > Mathf.Epsilon) && freeLookReturnIterator > freeLookReturnDelay / 4)) {
            freeLookReturnIterator = freeLookReturnDelay;
        } else {
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
        if (control.swapShoulders) {
            targetingRightShoulder = !targetingRightShoulder;
        }

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

    void TargetStandardPosition ()
    {
        Vector3 normPos = target.transform.position + rotation * offset.normalized * -zoom; //Regualr camera position if no obstruction is there
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
        }
        else
        {
            standardPosition = normPos; //In case no obstruction, use normal position
        }
    }

    void TargetRotation () {
        TargetStandardRotations ();
        TargetShoulderRotations ();

        targetRotation = Quaternion.Lerp (standardRotation, shoulderRotation, aimingBlend);
    }

    void TargetStandardRotations () {
        standardRotation = Quaternion.LookRotation (target.transform.position - transform.position);
    }

    void TargetShoulderRotations () {
        shoulderRotation = targetingRightShoulder ? rightShoulder.rotation : leftShoulder.rotation;
        shoulderRotation = Quaternion.Lerp (shoulderRotation, Quaternion.LookRotation (cam.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 50)) - transform.position), aimCameraSensitivityMultiplier);
    }

    void UpdateCursor () {
        if (control.freeMouse) {
            mouseReleased = !mouseReleased;
        }

        if (mouseReleased || control.aiming) {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } 
    }

    void ApplyTransform () {
        transform.position = Vector3.Lerp (transform.position, targetPosition, movementInterpolationSpeed);
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

    public void StartCutscene () {
        active = false;
    }

    public void StopCutscene () {
        active = true;
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}
