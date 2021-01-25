﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerMechanics : MonoBehaviour
{
    [Header("Vertical Movement")]
    [SerializeField] float jumpSpeed = 7f;
    [SerializeField] float smallJumpMod = 3f; //For double jump
    [SerializeField] float fallingMod = 4f; //Speed of falling
    [SerializeField] int hasJumped = 0;
    [SerializeField] Vector3 jumpDir = Vector3.up;

    [Space]

    [Header("Horizontal Movement")]
    [SerializeField] float normalSpeed = 3f;
    [SerializeField] float crouchMod;
    [SerializeField] float currentSpeed;

    [Space]

    [Header("Camera")]
    [SerializeField] float cameraSensitivity = 1f;
    [SerializeField] float xRotation = 0.0f;
    [SerializeField] float yRotation = 0.0f;

    [Space]

    [Header("Booleans")]
    public bool isWalking;
    public bool isCrouching;
    public bool isJumping;
    public bool isFalling;
    public bool isIdle;
    public bool isLanding;

    GameObject cam; //Camera

    Rigidbody rb; //Player's rigidbody

    CapsuleCollider playerCol; //The collider of the player
    PlayerCollisions collisions; //Controls collision interactions
    PlayerControlMapping control; //The control map of the player

    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        rb = GetComponent<Rigidbody>();
        playerCol = GetComponent<CapsuleCollider>();
        control = GetComponent<PlayerControlMapping>();
        collisions = GetComponent<PlayerCollisions>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //CameraMovement();
        Walk();
        Jump();
    }

    void CameraMovement()
    {
        yRotation += control.horizontalAim;
        xRotation -= control.verticalAim;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        cam.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
    }

    void Walk()
    {
        if(!control.crouching)
        {
          rb.velocity = new Vector3(control.xMove*normalSpeed,
          rb.velocity.y, control.vMove*normalSpeed);
        }
        else
        {
          rb.velocity = new Vector3(control.xMove*normalSpeed*crouchMod,
          rb.velocity.y, control.vMove*normalSpeed*crouchMod);
        }
    }

    void Jump()
    {
        if(control.jumpOn && hasJumped < 2) //If player presses up
        {
            //rb.AddForce(jumpDir*jumpSpeed, ForceMode2D.Impulse);
            rb.velocity = jumpDir*jumpSpeed;
            isJumping = true;
            hasJumped += 1;
        }
        if(rb.velocity.y < 0) //If player is falling
        {
            rb.velocity += jumpDir * Physics.gravity.y * (fallingMod - 1) * Time.deltaTime;
            isJumping = false;
            isFalling = true;
        }
        else if(rb.velocity.y > 0 && !control.jumpOn) //If the player is in the air and jumps again
        {
            rb.velocity += jumpDir * Physics.gravity.y * (smallJumpMod - 1) * Time.deltaTime;
            isFalling = false;
            isJumping = true;
        }
        if(rb.velocity.y == 0)
        {
            hasJumped = 0;
            isLanding = true;
        }
    }

}
