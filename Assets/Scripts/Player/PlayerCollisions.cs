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
    [SerializeField] float hitRange = 20f;

    [Space]

    [Header ("Masks & Transforms")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask interactionMask;
    [SerializeField] LayerMask opaqueMask;
    [SerializeField] LayerMask gatewayMask;
    [SerializeField] Transform groundTransform;
    [SerializeField] Transform visionTransform;

    Camera cam;

    void Start()
    {
        groundTransform = transform.Find("GroundTransform");
        visionTransform = transform.Find("VisionTarget");
        interactionMask = LayerMask.GetMask("Interactable");
        opaqueMask = LayerMask.GetMask("Opaque Object");
        cam = Camera.main;
    }

    public bool CheckGround () {
        return Physics.CheckBox (groundTransform.position, collisionDetectionBox, groundTransform.rotation, groundMask);
    }

    public bool CheckInteract () {
        Collider[] interactables = Physics.OverlapSphere(visionTransform.position, interactRadius, interactionMask);
        for (int i = 0; i < interactables.Length; i++) {
            if (QueryInteractable(interactables[i].transform)) {
                return true;
            }
        }
        return false;
    }
    public Collider CheckAttack()
    {
        RaycastHit hit;
        Physics.Raycast(visionTransform.position, visionTransform.forward, out hit, hitRange);
        return hit.collider;
    }

    public bool CheckGateway () {
        Collider [] interactables = Physics.OverlapSphere (visionTransform.position, interactRadius, gatewayMask);
        return interactables.Length > 0;
    }

    public string GetGatewayText () {
        Collider [] interactables = Physics.OverlapSphere (visionTransform.position, interactRadius, gatewayMask);
        return interactables [0].gameObject.name;
    }

    // Returns true if either the interactable is visible to the camera or to the player character
    public bool QueryInteractable (Transform interactable) {
        Debug.DrawLine (cam.transform.position, interactable.position, Color.red);
        // Checks whether the interactable is within the camera's view frustum and unobstructed
        Vector3 viewportPoint = cam.WorldToViewportPoint (interactable.position);
        if (!Physics.Linecast (cam.transform.position, interactable.position, opaqueMask) && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1) {
            return true;
        }
        Debug.DrawLine (visionTransform.position, interactable.position, Color.green);
        // Checks whether the interactable is unobstructed
        if (!Physics.Linecast (visionTransform.position, interactable.position, opaqueMask)) {
            return true;
        }
        return false;
    }

    public Collider[] Interact() {
        // interactable must be a created at runtime because
        // it breaks the logic if values from previous Interact() are still stored in the same variable

        List<Collider> interactables = new List<Collider> (Physics.OverlapSphere (visionTransform.position, interactRadius, interactionMask));
        int iterator = 0;
        while (iterator < interactables.Count) {
            if (!QueryInteractable (interactables [iterator].transform)) {
                interactables.RemoveAt (iterator);
            } else {
                iterator++;
            }
        }

        return interactables.ToArray ();
    }

}
