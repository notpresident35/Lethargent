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
    [SerializeField] float cameraSensitivity = 3f;
    [SerializeField] float xRotation = 0.0f;
    [SerializeField] float yRotation = 0.0f;
    [SerializeField] Quaternion rotation;
    [SerializeField] float zoom = -3f;
    [SerializeField] float zoomAmount = 0.2f;

    PlayerControlMapping control;
    SceneData sData;
    GameObject camTransform;
    LayerMask obstacleLayer;
    RaycastHit ray;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camTransform = GameObject.Find("ObstacleCheck");
        obstacleLayer = LayerMask.GetMask("Obstacle");
        offset = transform.position - player.transform.position;
        control = player.GetComponent<PlayerControlMapping>();
        sData = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SceneData>();
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
        yRotation += control.horizontalAim * cameraSensitivity;
        xRotation += control.verticalAim * cameraSensitivity;
        xRotation = Mathf.Clamp(xRotation, -30, 30);

        rotation = Quaternion.Euler(-xRotation, yRotation, 0);
        //transform.eulerAngles = new Vector3(-xRotation,
        //transform.eulerAngles.y + yRotation, 0.0f);
    }

    void ColliderCheck()
    {
        Debug.DrawRay(camTransform.transform.position, player.transform.position);

        if(Physics.Raycast(camTransform.transform.position, camTransform.transform.forward, out ray,
        -camTransform.transform.localPosition.z - 0.5f, obstacleLayer))
        {
            transform.position = ray.point;
            Debug.Log("hit");
        }
        else
        {
            transform.position = player.transform.position + rotation * offset;
        }
        transform.LookAt(player.transform.position);
    }

    void Zoom()
    {
        if(control.scroll != 0)
        {
            zoom += control.scroll * zoomAmount; 
        }

        zoom = Mathf.Clamp(zoom, -7f, -1.5f);
        offset.z = zoom;
    }
}
