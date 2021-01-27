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

  [Header("Collision Checks")]
  [SerializeField] Collider[] onGround;
  [SerializeField] Collider[] interactable;
  [SerializeField] bool inAir;
  [SerializeField] Transform groundTransform;

  Rigidbody rb;
  //Text interactText;
  PlayerMechanics mechanics;

  void Awake()
  {
    rb = GetComponent<Rigidbody>();
    //interactText = GameObject.Find("InteractText").GetComponent<Text>();
    mechanics = GetComponent<PlayerMechanics>();
  }

  void Start()
  {
      groundTransform = transform.Find("GroundTransform");
      //interactText.gameObject.SetActive(false);
  }

  void Update()
  {
      CheckGround();
      CheckInteract();
  }

  void CheckGround()
  {
      onGround = Physics.OverlapSphere(groundTransform.position, collisionRadius, 1 << Statics.GroundLayer);

      if(onGround.Length != 0) //If the player is standing on ground
      {
          inAir = false;
          mechanics.isJumping = false;
      }
      else
      {
          inAir = true;
      }
  }

  void CheckInteract()
  {
      interactable = Physics.OverlapSphere(transform.position, interactRadius, 1 << Statics.InteractableLayer);

      if(interactable.Length != 0)
      {
            interactable [0].GetComponent<GenericInteractable> ().InteractEvent ();
         //interactText.gameObject.SetActive(true);
      }
      else
      {
          //interactText.gameObject.SetActive(false);
      }
  }

  public bool IsInAir() //Checks if currently in air
  {
      return inAir;
  }
  public bool IsOnGround()
  {
      return onGround != null;
  }

}
