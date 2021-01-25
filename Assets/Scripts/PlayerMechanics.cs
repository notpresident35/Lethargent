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
        Walk();
        Jump();
        SaveNLoad();
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
