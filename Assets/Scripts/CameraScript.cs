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

    [Header ("Zoom")]

    [SerializeField] float zoom = -3f;
    [SerializeField] float zoomOffsetMax = 7f;
    [SerializeField] float zoomOffsetMin = 1.5f;
    [SerializeField] float zoomAmount = 0.2f;

    [Header ("Collision")]

    [SerializeField] float detectionRadius = 1.5f;
    [SerializeField] float cameraDistanceBuffer = 0.2f;

    PlayerControlMapping control;
    SceneData sData;
    Transform camTransform;
    Vector3 camTargetPosition;
    //LayerMask obstacleLayer;
    RaycastHit ray;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform.Find ("Vision Target");
        camTransform = transform.Find ("ObstacleCheck");
        control = player.GetComponent<PlayerControlMapping>();
        sData = FindObjectOfType<SceneData> ();// GameObject.FindGameObjectWithTag("Canvas").GetComponent<SceneData>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.position = offset;
        xRotation = defaultRotation.x;
        yRotation = defaultRotation.y;
        zoom = offset.magnitude;
        offset = offset.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        Zoom ();
        DetectCollisions ();
        transform.position = camTargetPosition;
    }

    void Rotate()
    {
        yRotation += control.horizontalAim * cameraSensitivity; //Capture horizontal mouse movement
        xRotation += (invertYAxis ? -1 : 1) * control.verticalAim * cameraSensitivity; //Capture vertical mouse movement
        xRotation = Mathf.Clamp(xRotation, verticalRotationMin, verticalRotationMax); //Clamp vertical movement to certain angles

        rotation = Quaternion.Euler(-xRotation, yRotation, 0);
    }

    void DetectCollisions()
    {
        Vector3 normPos = target.transform.position + Quaternion.Euler (offset) * transform.forward * zoom; //Regualr camera position if no obstruction is there
        //Debug.DrawLine(normPos, target.transform.position, Color.red, 2);

        // Raycasts from the vision target back to the camera, to ensure that the first target hit is always the foremost
        if(Physics.SphereCast (target.transform.position, detectionRadius, normPos - target.transform.position,
        out ray, (target.transform.position - normPos).magnitude, 1 << Statics.ObstacleLayer))
        {
            // Repositions the camera in front of the obstacle
            // Places the camera at the distance of the rayuast impact along the original line,
            // to allow us to use a spherecast while keeping the camera from snapping to the edge of surfaces 
            camTargetPosition = (normPos - target.transform.position).normalized * 
                                 (ray.point - target.transform.position).magnitude * 
                                 (1 - cameraDistanceBuffer) + target.transform.position;
            //Debug.Log("hit");
        }
        else
        {
            camTargetPosition = normPos; //In case no obstruction, use normal position
        }
        transform.LookAt(target.transform.position); //Continue to look at the player
    }

    void Zoom()
    {
        if(control.scroll != 0) //If player is scrolling
        {
            zoom += control.scroll * zoomAmount; //Scroll is +1 or -1
        }

        zoom = Mathf.Clamp(zoom, -zoomOffsetMax, -zoomOffsetMin); //Clamp the zoom allowed
    }
}
