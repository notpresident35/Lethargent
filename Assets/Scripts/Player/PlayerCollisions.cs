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
    [SerializeField] Vector3 collisionDetectionBox;
    [SerializeField] float interactRadius = 3f;

    [Space]

    [Header ("Masks & Transforms")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask interactionMask;
    [SerializeField] LayerMask opaqueMask;
    [SerializeField] Transform groundTransform;
    [SerializeField] Transform visionTransform;

    GameObject cam;

    void Start()
    {
        groundTransform = transform.Find("GroundTransform");
        visionTransform = transform.Find("VisionTarget");
        interactionMask = LayerMask.GetMask("Interactable");
        opaqueMask = LayerMask.GetMask("Opaque Object");
        cam = Camera.main.gameObject;
    }

    public bool CheckGround () {
        return Physics.CheckBox (groundTransform.position, collisionDetectionBox, groundTransform.rotation, groundMask);
    }

    public bool CheckInteract () {
        Collider[] interactables = Physics.OverlapSphere(visionTransform.position, interactRadius, interactionMask);
        for(int i = 0; i < interactables.Length; i++)
        {
            if(!Physics.Linecast(visionTransform.position, interactables[i].transform.position, opaqueMask))
            {
                return true;
            }
        }
        return false;
    }

    public Collider[] Interact()
    {
        // interactable must be a created at runtime because
        // it breaks the logic if values from previous Interact() are still stored in the same variable

        return Physics.OverlapSphere(visionTransform.position, interactRadius, interactionMask);
    }

}
