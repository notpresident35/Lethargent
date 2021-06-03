using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PlayerMechanics : MonoBehaviour {
    // The only thing that can set this to false is a cutscene
    [SerializeField] bool active = true;

    [Header ("Vertical Movement")]
    [SerializeField] float jumpSpeed = 8f;
    [SerializeField] float jumpInputLenience = 0.5f;
    [SerializeField] float jumpingGravityMod = 3f; //For double jump
    [SerializeField] float fallingGravityMod = 4f; //Speed of falling
    [SerializeField] int maxJumps = 1;
    [SerializeField] int jumpCount = 0;
    [SerializeField] Vector3 jumpDir = Vector3.up;

    [Space]

    [Header ("Horizontal Movement")]
    [SerializeField] float normalSpeed = 10f;
    [SerializeField] float turnSpeed = 0.1f;
    [SerializeField] float crouchMod = 0.5f;
    [SerializeField] float currentSpeed;
    [SerializeField] float aimRotationSpeed;

    [Space]

    [Header ("Crouching")]
    [SerializeField] float standingHeight;
    [SerializeField] float crouchingHeight;

    [Space]

    [Header ("Items")]
    [SerializeField] bool itemHeld;
    [SerializeField] float cooldownTimer = 0.0f;
    [SerializeField] float itemUseDelay = 2.0f;
    [SerializeField] Transform grabber;
    [SerializeField] Transform heldItem;
    [SerializeField] ItemData heldItemData;

    [Space]

    [Header ("Cutscenes")]
    [SerializeField] GameObject Gateway1;
    [SerializeField] Transform Cutscene2BossGrabber;
    [SerializeField] Transform Cutscene2BossDesk;
    [SerializeField] GameObject AssigmentItem;

    [Space]

    [Header ("SFX")]
    [SerializeField] AudioClip JumpSFX;

    [Space]

    [Header ("Booleans")]
    public bool isWalking;
    public bool isCrouching;
    public bool isJumping;
    public bool isFalling;
    public bool isIdle;
    public bool joystickControls;

    GameObject cam; //Camera
    PlayerCollisions collisions; //Controls collision interactions
    InputManager control; //The control map of the player
    CharacterController controller;
    Transform model; //Player's model
    Transform camCache;
    Animator anim;
    Vector3 movement = Vector3.zero;

    float smoothTime;
    float jumpInputCache;
    Vector3 jumpVelocity = Vector3.zero;
    string currentAnimState;
    bool wasHoldingItem;

    void Awake () {
        cam = Camera.main.gameObject;
        camCache = new GameObject ("CameraCache").transform;
        //restartText = GameObject.Find("RestartText").GetComponent<Text>();
        control = FindObjectOfType<InputManager> ();
        controller = GetComponent<CharacterController> ();
        collisions = GetComponent<PlayerCollisions> ();
        anim = GetComponent<Animator> ();
        model = transform.Find ("Model");
    }

    // Start is called before the first frame update
    void Start () {
        //restartText.gameObject.SetActive(false);
        currentSpeed = normalSpeed;
        jumpVelocity.y = -6f; // Acts like a bit of gravity while moving down slopes
    }

    // Update is called once per frame
    void Update () {
        anim.applyRootMotion = active;
        if (!active) { return; }
        Grab ();
        Interact ();
        //SaveNLoad ();
        Crouch ();
        UseItem();
    }

    void FixedUpdate () {
        if (!active) { return; }
        Walk ();
        Jump ();
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
        controller.Move (movement * Time.fixedDeltaTime);

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

        if (movement.magnitude < Mathf.Epsilon) {
            if (control.crouching) {
                SetAnim ("CrouchingIdle");
            } else {
                SetAnim ("Idle");
            }
        } else {
            if (control.crouching) {
                SetAnim ("CrouchingWalkingForward");
            } else {
                SetAnim ("RunningForward");
            }
        }
    }

    void Jump () {
        if (control.jumpOn) {
            jumpInputCache = jumpInputLenience;
        } else {
            jumpInputCache -= Time.deltaTime;
        }

        if (jumpInputCache > 0 && jumpCount < maxJumps) //If player presses up
        {
            jumpVelocity = jumpDir.normalized * jumpSpeed;
            jumpInputCache = 0;
            isJumping = true;
            jumpCount++;
            AudioManager.Play2DSound (JumpSFX, Statics.SFXMixerGroupName, 1, false, Random.Range (0.95f, 1.05f));
        }

        if (jumpVelocity.y > Mathf.Epsilon) // Player is jumping up; fall slower if still holding the jump button
        {
            jumpVelocity += Physics.gravity * (control.jumping ? jumpingGravityMod : fallingGravityMod) * Time.fixedDeltaTime;
            isFalling = false;
            isJumping = true;
        } else if (collisions.CheckGround ()) { // Player is on the ground
            jumpVelocity *= 0.9f;
            jumpCount = 0;
            isFalling = false;
            isJumping = false;
        } else // Player is falling down
          {
            jumpVelocity += Physics.gravity * fallingGravityMod * Time.fixedDeltaTime;
            isJumping = false;
            isFalling = true;
        }

        controller.Move (jumpVelocity * Time.fixedDeltaTime);
    }

    void Crouch () {
        controller.height = control.crouching ? crouchingHeight : standingHeight;
        controller.center = new Vector3(0, (control.crouching ? crouchingHeight : standingHeight) / 2, 0);
    }

    void Interact () {
        if (control.interact && collisions.CheckInteract()) {
            Collider [] cols = collisions.Interact (); //Get interactables
            if (cols.Length != 0) {
                int closestIndex = 0;
                for (int i = 0; i < cols.Length; i++) {
                    if ((cols[i].transform.position - transform.position).magnitude <= (cols [closestIndex].transform.position - transform.position).magnitude) {
                        closestIndex = i;
                    }
                }
                cols[closestIndex].GetComponent<GenericInteractable> ().Interact (); //Trigger interactable
                if (cols[closestIndex].GetComponent<Item> ()) {
                    itemHeld = true;
                    heldItemData = cols [closestIndex].GetComponent<Item> ().data;
                    heldItem = cols [closestIndex].transform;
                    heldItem.transform.parent = grabber;
                    heldItem.localPosition = Vector3.zero;
                    heldItem.localRotation = Quaternion.identity;
                    cols [closestIndex].enabled = false;
                    // TODO: Refactor
                    if (Gateway1.activeSelf) {
                        Gateway1.SetActive (false);
                    }
                }
            }
        }
    }

    void Grab () {
        /*if (itemHeld) {
            heldItem.position = hand.position;
            heldItem.rotation = hand.rotation;
        }*/
        if (itemHeld && !wasHoldingItem) {
            anim.Play ("ItemHeld" + (heldItemData.oneHanded ? "OneHand" : ""), 2);
        }
        if (!itemHeld && wasHoldingItem) {
            anim.Play (currentAnimState, 2);
        }
        wasHoldingItem = itemHeld;
        if (heldItem && !heldItem.gameObject.activeInHierarchy) {
            heldItem.gameObject.SetActive (true);
        }
    }

    void UseItem () {
        if(control.clicking && heldItemData && cooldownTimer >= itemUseDelay) {

            if (heldItemData.type == ItemData.Type.Weapon) {
                Collider target = collisions.CheckAttack ();
                if (target != null) {
                    target.GetComponent<Enemy> ().Damage (((WeaponData) heldItemData).damage);
                }
            }
            cooldownTimer = 0f;
        }
        cooldownTimer += Time.deltaTime;
    }

    // Not needed anymore; just debug code
    /*
    void SaveNLoad()
    {
        if (control.save)
        {
            SaveLoad.Save (0);
        }

        //-----------------------------------------------------------------

        if (control.load)
        {
            SaveLoad.Load (0);
            if (LevelManager.current.playerData.finishedGame)
            {
                //restartText.gameObject.SetActive(true);
                if (control.load)
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
    }*/

    void SetAnim (string animState) {
        if (currentAnimState == animState) { return; }
        anim.Play (animState, 0);
        if (!heldItem || !itemHeld) {
            anim.Play (animState, 2);
        }
        currentAnimState = animState;
    }

    private void OnEnable() {
        CutsceneManager.CutsceneStart += StartCutscene;
        CutsceneManager.CutsceneStop += StopCutscene;
        Menu.GameStart += StartGame;
        SaveLoad.SyncDataForSave += SyncDataForSave;
        SaveLoad.SyncDataOnLoad += SyncDataOnLoad;
    }

    private void OnDisable() {
        CutsceneManager.CutsceneStart -= StartCutscene;
        CutsceneManager.CutsceneStop -= StopCutscene;
        Menu.GameStart -= StartGame;
        SaveLoad.SyncDataForSave -= SyncDataForSave;
        SaveLoad.SyncDataOnLoad -= SyncDataOnLoad;
    }

    public void StartCutscene() {
        active = false;
    }

    public void StopCutscene() {
        active = true;
    }

    public void Cutscene2Event1 () {
        itemHeld = false;
        heldItem.parent = Cutscene2BossGrabber;
        heldItem.localPosition = Vector3.zero;
        heldItem.localRotation = Quaternion.identity;
    }

    public void Cutscene2Event2 () {
        heldItem.parent = Cutscene2BossDesk;
        heldItem.localPosition = Vector3.zero;
        heldItem.localRotation = Quaternion.identity;
        heldItem.GetComponent<Collider> ().enabled = false;
        heldItem = null;
        heldItemData = null;
    }

    public void Cutscene2Event3 () {
        AssigmentItem.SetActive (true);
    }

    public void Cutscene3Event1 () {
        heldItem.GetChild (0).gameObject.SetActive (true);
        heldItem.GetChild (1).gameObject.SetActive (false);
    }

    // This PERMANENTLY DELETES the player's currently held item (by moving it 1000 units away to avoid breaking the save/load system)
    // Only use this if you KNOW FOR SURE what the player is holding (pretty much only for use in cutscenes and certain mission objectives)
    public void RemoveHeldItem () {
        if (heldItem) {
            itemHeld = false;
            heldItem.parent = null;
            heldItem.position = new Vector3 (0, 1000, 0);
            heldItem.rotation = Quaternion.identity;
            heldItemData = null;
            heldItem = null;
        } else {
            Debug.LogError ("Why on earth are you using this if the player isn't holding an item?!?");
        }
    }

    public void GotHit(int hp)
    {
        LevelManager.current.playerData.health -= hp;
    }

    void StartGame () {
        active = true;
    }

    void SyncDataForSave () {
        LevelManager.current.playerData.sceneID = SceneManager.GetActiveScene ().buildIndex;
        LevelManager.current.playerData.playerPos = transform.position;
        LevelManager.current.playerData.playerRot = transform.rotation;
        if (heldItem) {
            LevelManager.current.playerData.heldItemUniqueID = heldItem.GetComponent<Item> ().UniqueID;
        } else {
            LevelManager.current.playerData.heldItemUniqueID = -1;
        }
    }

    // TODO: Maybe optimize so that SetUnHeld doesn't have to be called on *every* item? 
    void SyncDataOnLoad () {
        transform.position = LevelManager.current.playerData.playerPos;
        transform.rotation = LevelManager.current.playerData.playerRot;

        // Drop and reset currently held item
        if (itemHeld) {
            itemHeld = false;
            heldItem.position = LevelManager.current.playerData.itemsPositions [heldItem.GetComponent<Item> ().UniqueID];
            heldItem.rotation = LevelManager.current.playerData.itemsRotations [heldItem.GetComponent<Item> ().UniqueID];
            heldItem.GetComponent<Collider> ().enabled = true;
            heldItem.GetComponent<Item> ().SetUnHeld ();
            heldItem.transform.parent = null;
            heldItemData = null;
            heldItem = null;
        }

        if (LevelManager.current.playerData.heldItemUniqueID > 0) {
            foreach (Item item in Resources.FindObjectsOfTypeAll (typeof (Item))) {
                if (Mathf.Abs (LevelManager.current.playerData.heldItemUniqueID - item.UniqueID) < Mathf.Epsilon) {
                    itemHeld = true;
                    heldItemData = item.data;
                    heldItem = item.transform;
                    heldItem.transform.parent = grabber;
                    heldItem.localPosition = Vector3.zero;
                    heldItem.localRotation = Quaternion.identity;
                    heldItem.GetComponent<Collider> ().enabled = false;
                    item.SetHeld ();
                    break;
                }
            }
            if (!heldItem) {
                Debug.LogError ("Couldn't find item with saved ID!");
            }
            LevelManager.current.playerData.heldItemUniqueID = heldItem.GetComponent<Item> ().UniqueID;
        }
    }
}
