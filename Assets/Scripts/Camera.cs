using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Camera : MonoBehaviour
{
    GameObject player;

    [Header("Camera Coordinates")]
    [SerializeField] float minX; //Booundaries of level
    [SerializeField] float maxX;
    [SerializeField] float minY;
    [SerializeField] float maxY;
    [SerializeField] Vector3 offset;
    [SerializeField] float cameraSensitivity = 3f;
    [SerializeField] float xRotation = 0.0f;
    [SerializeField] float yRotation = 0.0f;

    PlayerControlMapping control;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
        control = player.GetComponent<PlayerControlMapping>();
        //sData = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SceneData>();

        /*minX = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SceneData>().minX;
        maxX = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SceneData>().maxX;
        minY = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SceneData>().minY;
        maxY = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SceneData>().maxY;*/
    }

    // Update is called once per frame
    void Update()
    {
        /*float horizontal = control.horizontalAim * cameraSensitivity;
        player.transform.Rotate(0, horizontal, 0);

        float angle = player.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, angle, 0);*/

        yRotation = control.horizontalAim * cameraSensitivity;
        xRotation += control.verticalAim * cameraSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90, 60);

        transform.eulerAngles = new Vector3(-xRotation,
        transform.eulerAngles.y + yRotation, 0.0f);

        transform.position = player.transform.position + offset;
        /*if (maxX < newPos.x)
            newPos.x = maxX;
        else if (newPos.x < minX)
            newPos.x = minX;

        if (maxY < newPos.y)
            newPos.y = maxY;
        else if (newPos.y < minY)
            newPos.y = minY;*/

        //transform.LookAt(player.transform);
    }
}
