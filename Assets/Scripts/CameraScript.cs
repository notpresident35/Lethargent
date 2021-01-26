using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour
{
    GameObject player;

    [Header("Camera Coordinates")]

    [SerializeField] float minX; //Booundaries of level
    [SerializeField] float maxX;
    [SerializeField] float minY;
    [SerializeField] float maxY;
    [SerializeField] Vector3 offset = new Vector3(0, 1, -3);

    [Header ("Rotation")]

    [SerializeField] float cameraSensitivity = 3f;
    [SerializeField] float verticalRotationMax;
    [SerializeField] float verticalRotationMin;
    [SerializeField] float xRotation = 0.0f;
    [SerializeField] float yRotation = 0.0f;
    [SerializeField] Quaternion rotation;

    [Header ("Zoom")]

    [SerializeField] float zoom = -3f;
    [SerializeField] float zoomAmount = 0.2f;

    PlayerControlMapping control;
    SceneData sData;
    Transform camTransform;
    //LayerMask obstacleLayer;
    RaycastHit ray;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camTransform = transform.Find ("ObstacleCheck");
        //obstacleLayer = LayerMask.GetMask("Obstacle");
        offset = transform.position - player.transform.position;
        control = player.GetComponent<PlayerControlMapping>();
        sData = FindObjectOfType<SceneData> ();// GameObject.FindGameObjectWithTag("Canvas").GetComponent<SceneData>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        ColliderCheck();
        Zoom();
    }

    void Rotate()
    {
        yRotation += control.horizontalAim * cameraSensitivity; //Capture horizontal mouse movement
        xRotation += control.verticalAim * cameraSensitivity; //Capture vertical mouse movement
        xRotation = Mathf.Clamp(xRotation, -30, 30); //Clamp vertical movement to certain angles

        rotation = Quaternion.Euler(-xRotation, yRotation, 0);
    }

    void ColliderCheck()
    {
        Vector3 normPos = player.transform.position + rotation * offset; //Regualr camera position if no obstruction is there
        //Debug.DrawRay(normPos, player.transform.position);

        if(Physics.Raycast(normPos, player.transform.position - normPos,
        out ray, (player.transform.position - normPos).magnitude, 1 << Statics.ObstacleLayer))
        {
            transform.position = (ray.point - player.transform.position) * 0.8f + player.transform.position; //Get camera closer in case obstruction between camera and player
            Debug.Log("hit");
        }
        else
        {
            transform.position = normPos; //In case no obstruction, use normal position
        }
        transform.LookAt(player.transform.position); //Continue to look at the player
    }

    void Zoom()
    {
        if(control.scroll != 0) //If player is scrolling
        {
            zoom += control.scroll * zoomAmount; //Scroll is +1 or -1
        }

        zoom = Mathf.Clamp(zoom, -7f, -1.5f); //Clamp the zoom allowed
        offset.z = zoom; //Update offset with zoom
    }
}
