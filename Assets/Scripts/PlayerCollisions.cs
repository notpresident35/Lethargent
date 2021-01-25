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

  [Space]

  [Header("Layers")]
  [SerializeField] LayerMask groundLayer;
  [SerializeField] LayerMask interactLayer;

  Rigidbody rb;

  PlayerMechanics mechanics;

  void Awake()
  {
    rb = GetComponent<Rigidbody>();
    mechanics = GetComponent<PlayerMechanics>();
    groundLayer = LayerMask.GetMask("Ground");
    interactLayer = LayerMask.GetMask("Interact");
  }

  void Start()
  {
      groundTransform = transform.Find("GroundTransform");
  }

  void Update()
  {
      CheckGround();
      CheckInteract();
  }

  void CheckGround()
  {
      onGround = Physics.OverlapSphere(groundTransform.position, collisionRadius, groundLayer);

      if(onGround != null) //If the player is standing on ground
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
      interactable = Physics.OverlapSphere(transform.position, interactRadius, interactLayer);

      if(interactable != null)
      {
         //TODO
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
