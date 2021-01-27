using System.Collections;
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
    [SerializeField] float turnSpeed = 0.1f;
    [SerializeField] float crouchMod = 0.5f;
    [SerializeField] float currentSpeed;

    [Space]

    [Header("Booleans")]
    public bool isWalking;
    public bool isCrouching;
    public bool isJumping;
    public bool isFalling;
    public bool isIdle;
    public bool isLanding;


    GameObject cam; //Camera
    //Text restartText; //Restart again text
    Rigidbody rb; //Player's rigidbody

    CapsuleCollider playerCol; //The collider of the player
    PlayerCollisions collisions; //Controls collision interactions
    PlayerControlMapping control; //The control map of the player
    CharacterController controller;
    Transform model;

    float smoothTime;

    void Awake()
    {
        cam = Camera.main.gameObject;
        //restartText = GameObject.Find("RestartText").GetComponent<Text>();
        rb = GetComponent<Rigidbody>();
        playerCol = GetComponent<CapsuleCollider>();
        control = GetComponent<PlayerControlMapping>();
        collisions = GetComponent<PlayerCollisions>();
        controller = GetComponent<CharacterController> ();
        model = transform.Find ("Model");
    }
    // Start is called before the first frame update
    void Start()
    {
        //restartText.gameObject.SetActive(false);
        currentSpeed = normalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        Jump();
        SaveNLoad();
    }

    void Walk()
    {
        if(!control.crouching)
        {
            currentSpeed = normalSpeed;
        }
        else
        {
            currentSpeed = normalSpeed * crouchMod;
        }

        // Movement
        Vector3 dir = new Vector3(control.xMove, 0.0f, control.vMove);
        Vector3 camRot = cam.transform.rotation.eulerAngles;
        camRot.x = 0; // Ignore vertical camera rotation
        dir = Quaternion.Euler (camRot) * dir; //Change direction based on where camera is facing
        Vector3 movement = rb.velocity;
        movement.x = dir.x * currentSpeed;
        movement.z = dir.z * currentSpeed;
        rb.velocity = movement;

        // Rotate smoothly to face movement
        if (dir.magnitude > Mathf.Epsilon) {
            float angle = Mathf.Atan2 (dir.x, dir.z) * Mathf.Rad2Deg;
            angle = Mathf.SmoothDampAngle (transform.eulerAngles.y, angle, ref smoothTime, turnSpeed);
            transform.rotation = Quaternion.Euler (0, angle, 0);
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
        if(rb.velocity.y < Mathf.Epsilon) //If player is falling
        {
            rb.velocity += jumpDir * Physics.gravity.y * (fallingMod - 1) * Time.deltaTime;
            isJumping = false;
            isFalling = true;
        }
        else if(rb.velocity.y > Mathf.Epsilon) //If the player is in the air and jumps again
        {
            if (!control.jumpOn) {
                rb.velocity += jumpDir * Physics.gravity.y * (smallJumpMod - 1) * Time.deltaTime;
                isFalling = false;
                isJumping = true;
            }
        }
        else
        {
            hasJumped = 0;
            isLanding = true;
        }
    }

    void SaveNLoad()
    {
        if(control.save)
        {
            Scene scene = SceneManager.GetActiveScene();
            LevelManager.current.playerData.sceneID = scene.buildIndex;
            LevelManager.current.playerData.playerPosX = transform.position.x;
            LevelManager.current.playerData.playerPosY = transform.position.y;
            LevelManager.current.playerData.playerPosZ = transform.position.z;
            SaveLoad.Save();
        }

        //-----------------------------------------------------------------

        if(control.load)
        {
            SaveLoad.Load();
            if(LevelManager.current.playerData.finishedGame)
            {
                //restartText.gameObject.SetActive(true);
                if(control.load)
                {
                    goto loading;
                }
                else
                {
                    return;
                }
            }
            loading:
                LevelManager.current.isSceneBeingLoaded = true;
                int whichScene = LevelManager.current.playerData.sceneID;
                SceneManager.LoadScene(whichScene);

                float t_x = LevelManager.current.playerData.playerPosX;
                float t_y = LevelManager.current.playerData.playerPosY;
                float t_z = LevelManager.current.playerData.playerPosZ;

                transform.position = new Vector3(t_x, t_y, t_z);
        }
    }

}
