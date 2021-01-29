using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour
{
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

    [Header ("Effects")]

    [SerializeField] float freeLookReturnDelay = 2;
    float freeLookReturnIterator;
    [SerializeField] float movementInterpolationSpeed = 0.05f;
    [SerializeField] float maxMovementSpeed = 1;
    /*[SerializeField] float currentMovementSpeed;
    [SerializeField] float movementAcceleration;*/
    [SerializeField] float rotationInterpolationFactor = 0.2f;
    bool wasFreeLooking;
    bool wasAiming;
    bool wasInCutscene;

    [Header ("Collision")]

    [SerializeField] float detectionRadius = 1.5f;
    [SerializeField] float shoulderDetectionRadius = 0.5f;
    [SerializeField] float cameraDistanceBuffer = 0.2f;

    PlayerControlMapping control;
    SceneData sData;
    Transform camTransform;
    Camera cam;
    //LayerMask obstacleLayer;
    RaycastHit ray;
    Vector3 targetPosition;
    Quaternion targetRotation;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform.Find ("CameraTarget (Default)");
        leftShoulder = player.transform.Find ("CameraTarget (Left Shoulder)");
        rightShoulder = player.transform.Find ("CameraTarget (Right Shoulder)");
        camTransform = transform.Find ("ObstacleCheck");
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
        if (CutsceneManager.Active) { wasInCutscene = true; return; }
        GetRotationInput();
        Zoom ();
        if (control.aiming) {
            TargetShoulderPositions ();
        } else {
            TargetStandardPosition ();
        }
        TargetRotation ();
        ApplyTransform ();
        ShakeScreen ();
        wasInCutscene = false;
    }

    void GetRotationInput()
    {
        // Ignore mouse input if player isn't aiming or freelooking
        if (control.aiming || control.freeLooking) {
            rotationDelta.y = control.horizontalAim * cameraSensitivity;
            rotationDelta.x = (invertYAxis ? -1 : 1) * control.verticalAim * cameraSensitivity;
        } else {
            rotationDelta = Vector3.zero;
        }

        if (!control.aiming) {
            if (control.freeLooking || freeLookReturnIterator < freeLookReturnDelay) {
                yRotation += rotationDelta.y; //Capture horizontal mouse movement
                xRotation += rotationDelta.x; //Capture vertical mouse movement
                xRotation = Mathf.Clamp (xRotation, verticalRotationMin, verticalRotationMax); //Clamp vertical movement to certain angles
            } else {
                if (wasFreeLooking || wasAiming) {
                    //print ("Reset vertical rotation");
                    xRotation = defaultRotation.x;
                }
                yRotation = player.transform.rotation.eulerAngles.y;// * Mathf.Rad2Deg;
            }
            
            rotation = Quaternion.Euler (-xRotation, yRotation, 0);
        }

        wasFreeLooking = freeLookReturnIterator < freeLookReturnDelay;
        wasAiming = control.aiming;

        if (control.freeLooking) {
            freeLookReturnIterator = 0;
        } else if (control.aiming) {
            freeLookReturnIterator = freeLookReturnDelay;
        } else {
            freeLookReturnIterator += Time.deltaTime;
            freeLookReturnIterator = Mathf.Clamp (freeLookReturnIterator, 0, freeLookReturnDelay);
        }
    }

    void TargetShoulderPositions () {
        if (control.swapShoulders) {
            targetingRightShoulder = !targetingRightShoulder;
        }

        Vector3 normPos = targetingRightShoulder ? rightShoulder.position : leftShoulder.position;
        //Debug.DrawRay(target.transform.position, normPos - target.transform.position, Color.red, 2);

        // SphereCasts back a bit from the camera, making sure that the camera does not get clip into a wall if the player backs up
        if (Physics.SphereCast (normPos, shoulderDetectionRadius, -target.transform.forward, out ray, shoulderDetectionRadius, 1 << Statics.ObstacleLayer)) {
            // Repositions the camera in front of the obstacle
            // Places the camera at the distance of the rayuast impact along the original line,
            // to allow us to use a spherecast while keeping the camera from snapping to the edge of surfaces 
            targetPosition = normPos - target.transform.forward * (ray.point - normPos).magnitude * (1 - cameraDistanceBuffer);
            //Debug.Log("hit");
        } else {
            targetPosition = normPos; //In case no obstruction, use normal position
        }
    }

    void TargetStandardPosition()
    {
        Vector3 normPos = target.transform.position + rotation * offset.normalized * -zoom; //Regualr camera position if no obstruction is there
        //Debug.DrawRay(target.transform.position, normPos - target.transform.position, Color.red, 2);

        // SphereCasts from the vision target back to the camera, to ensure that the first target hit is always the foremost
        if(Physics.SphereCast (target.transform.position, detectionRadius, normPos - target.transform.position, out ray, -zoom, 1 << Statics.ObstacleLayer)) {
            // Repositions the camera in front of the obstacle
            // Places the camera at the distance of the rayuast impact along the original line,
            // to allow us to use a spherecast while keeping the camera from snapping to the edge of surfaces 
            targetPosition = (normPos - target.transform.position).normalized * 
                             (ray.point - target.transform.position).magnitude * 
                             (1 - cameraDistanceBuffer) + target.transform.position;
            //Debug.Log("hit");
        }
        else
        {
            targetPosition = normPos; //In case no obstruction, use normal position
        }
    }
    void TargetRotation () {
        if (control.aiming) {
            // Y-axis rotation should directly control the player's rotation, not the camera's
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            targetRotation = targetingRightShoulder ? rightShoulder.rotation : leftShoulder.rotation;
            targetRotation = Quaternion.Lerp (targetRotation, Quaternion.LookRotation (cam.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 50)) - transform.position), aimCameraSensitivityMultiplier);
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            targetRotation = Quaternion.LookRotation (target.transform.position - transform.position);
            //transform.LookAt (target.transform.position); //Continue to look at the player
        }
    }

    void Zoom()
    {
        if(control.scroll != 0) //If player is scrolling
        {
            zoom += control.scroll * zoomAmount; //Scroll is +1 or -1
        }

        zoom = Mathf.Clamp(zoom, -zoomOffsetMax, -zoomOffsetMin); //Clamp the zoom allowed
    }

    void ApplyTransform () {
        if (wasInCutscene) {
            transform.position = targetPosition;
            transform.rotation = targetRotation;
        } else {
            Vector3 newPos = Vector3.Lerp (transform.position, targetPosition, movementInterpolationSpeed);
            transform.position += (newPos - transform.position).normalized * Mathf.Clamp ((newPos - transform.position).magnitude, 0f, maxMovementSpeed);
            transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, rotationInterpolationFactor);
        }
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
}
