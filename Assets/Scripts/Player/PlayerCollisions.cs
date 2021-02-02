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
    [SerializeField] float interactRadius = 1f;

    [Space]

    [Header ("Collision Checks")]
    [SerializeField] LayerMask collisionMask;
    [SerializeField] LayerMask interactionMask;
    [SerializeField] Transform groundTransform;

    void Start ()
    {
        groundTransform = transform.Find("GroundTransform");
        //interactText.gameObject.SetActive(false);
    }

    public bool CheckGround () {
        return Physics.CheckSphere (groundTransform.position, collisionRadius, collisionMask);
    }

    public bool Interact () {
        // interactable must be a created at runtime because
        // it breaks the logic if values from previous Interact() are still stored in the same variable
        Collider [] interactable = Physics.OverlapSphere (transform.position, interactRadius, interactionMask);
        if (interactable.Length != 0) {
            interactable [0].GetComponent<GenericInteractable> ().InteractEvent ();
            return true;
        }
        return false;
    }
}
