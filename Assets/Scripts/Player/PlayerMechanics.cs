using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerMechanics : MonoBehaviour
{
    // The only thing that can set this to false is a cutscene
    [SerializeField] bool active = true;

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
    [SerializeField] float aimRotationSpeed;

    [Space]

    [Header("Booleans")]
    public bool isWalking;
    public bool isCrouching;
    public bool isJumping;
    public bool isFalling;
    public bool isIdle;
    public bool isLanding;
    public bool joystickControls;


    GameObject cam; //Camera
    //Text restartText; //Restart again text
    Rigidbody rb; //Player's rigidbody

    CapsuleCollider playerCol; //The collider of the player
    PlayerCollisions collisions; //Controls collision interactions
    PlayerControlMapping control; //The control map of the player
    CharacterController controller;
    Transform model;
    Transform camCache;
    Vector3 movement = Vector3.zero;

    float smoothTime;

    void Awake()
    {
        cam = Camera.main.gameObject;
        camCache = new GameObject ("CameraCache").transform;
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
        if (!active) { return; }
        Walk ();
        Jump();
        SaveNLoad();
    }

    void Walk () {
        if (!control.crouching) {
            currentSpeed = normalSpeed;
        } else {
            currentSpeed = normalSpeed * crouchMod;
        }

        // Movement
        Vector3 dir = new Vector3 (control.xMove, 0.0f, control.vMove);
        Vector3 camRot = cam.transform.rotation.eulerAngles;

        // Change direction based on camera angle
        if (joystickControls) {
            // Only updates the reference angle for the camera's rotation if the player stops moving
            // This allows the player to hold s to move straight in one direction while the camera rotates 180 degrees
            // This also allows the player to intuitively move in, say, a circle while using a joystick
            if (dir.magnitude < Mathf.Epsilon) {
                camCache.position = cam.transform.position;
                camCache.rotation = cam.transform.rotation;
            }
            camRot = camCache.rotation.eulerAngles;
        }

        camRot.x = 0; // Ignore vertical camera rotation
        dir = (Quaternion.Euler (camRot) * dir).normalized;
        /*
        Vector3 xCamRot = camCacheX.rotation.eulerAngles;
        Vector3 yCamRot = camCacheY.rotation.eulerAngles;
        xCamRot.x = 0; // Ignore vertical camera rotation
        yCamRot.x = 0;
        Vector3 xDir = dir; //Change direction based on where camera is facing
        xDir.z = 0;
        xDir = Quaternion.Euler (xCamRot) * xDir;
        Vector3 yDir = dir; //Change direction based on where camera is facing
        yDir.x = 0;
        yDir = Quaternion.Euler (yCamRot) * yDir;
        dir = (xDir + yDir).normalized;*/

        movement = dir * currentSpeed;
        movement.y = rb.velocity.y;
        rb.velocity = movement;

        if (control.aiming) {
            // Rotate with mouse movement
            transform.Rotate (0, control.horizontalAim * aimRotationSpeed, 0);
        } else {
            // Rotate smoothly to face movement
            if (dir.magnitude > Mathf.Epsilon) {
                float angle = Mathf.Atan2 (dir.x, dir.z) * Mathf.Rad2Deg;
                angle = Mathf.SmoothDampAngle (transform.eulerAngles.y, angle, ref smoothTime, turnSpeed);
                transform.rotation = Quaternion.Euler (0, angle, 0);
            }
        }
    }

    
    /*private void OnDrawGizmos () {
        if (!Application.isPlaying) { return; }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere (camCacheX.position, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere (camCacheY.position, 0.5f);
    }*/

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

    private void OnEnable () {
        CutsceneManager.CutsceneStart += StartCutscene;
        CutsceneManager.CutsceneStop += StopCutscene;
    }

    private void OnDisable () {
        CutsceneManager.CutsceneStart -= StartCutscene;
        CutsceneManager.CutsceneStop -= StopCutscene;
    }

    public void StartCutscene () {
        active = false;
    }

    public void StopCutscene () {
        active = true;
    }
}
