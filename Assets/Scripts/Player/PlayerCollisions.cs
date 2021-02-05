using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class PlayerCollisions : MonoBehaviour
{
    [Header("Offsets of Player")]
    [SerializeField] float collisionRadius = 0.5f;
    [SerializeField] float interactRadius = 3f;

    [Space]

    [Header ("Masks & Transforms")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask interactionMask;
    [SerializeField] Transform groundTransform;
    [SerializeField] Transform visionTransform;

    GameObject cam;

    void Start()
    {
        groundMask = LayerMask.GetMask("Terrain");
        interactionMask = LayerMask.GetMask("Interactable");
        groundTransform = transform.Find("GroundTransform");
        visionTransform = transform.Find("VisionTarget");
        cam = Camera.main.gameObject;
    }

    public bool CheckGround()
    {
        return Physics.CheckSphere(groundTransform.position, collisionRadius, groundMask);
    }

    public bool CheckInteract () {
        return Physics.CheckSphere (visionTransform.position, interactRadius, interactionMask);
    }

    public Collider[] Interact()
    {
        // interactable must be a created at runtime because
        // it breaks the logic if values from previous Interact() are still stored in the same variable

        return Physics.OverlapSphere(visionTransform.position, interactRadius, interactionMask);
    }

}
