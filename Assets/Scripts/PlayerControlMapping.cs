using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlMapping : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float m_xMove;
    [SerializeField] float m_vMove;
    [SerializeField] float m_horizontalAim;
    [SerializeField] float m_verticalAim;
    [SerializeField] float m_scroll;
    [SerializeField] bool m_jumpOn;
    [SerializeField] bool m_crouching;
    [SerializeField] bool m_enter;
    [SerializeField] bool m_pause;
    [SerializeField] bool m_save;
    [SerializeField] bool m_load;

    [SerializeField] bool m_inputting = true; //For debugging only, revert to no value before publishing game

    // JUST A PLACEHOLDER TO REMOVE ERRORS
    // Start is called before the first frame update
    void Awake()
    {
         rb = GetComponent<Rigidbody>();
        CaptureInput ();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_inputting)
        {
            CaptureInput ();
        }
        else
        {
            CaptureDummyInput ();
        }
    }

    public void NoInput()
    {
        m_inputting = false; //Shuts off input for player
    }
    public void StartInput()
    {
        m_inputting = true; //Turns on input for player
    }

    public IEnumerator PauseInput(float delay) //Toggles input for delay amount
    {
        NoInput();
        yield return new WaitForSeconds(delay);
        StartInput();
    }

    void CaptureInput () {
        m_xMove = Input.GetAxis ("Horizontal");
        m_vMove = Input.GetAxis ("Vertical");
        m_horizontalAim = Input.GetAxis ("Mouse X");
        m_verticalAim = Input.GetAxis ("Mouse Y");
        m_scroll = Input.mouseScrollDelta.y;
        m_jumpOn = Input.GetButtonDown ("Jump");
        m_crouching = Input.GetButton ("Crouch");
        m_enter = Input.GetButtonDown ("Submit");
        m_pause = Input.GetButtonDown ("Pause");

        m_save = Input.GetKeyDown (KeyCode.F5);
        m_load = Input.GetKeyDown (KeyCode.F6);
    }

    void CaptureDummyInput () {
        m_xMove = 0;
        m_vMove = 0;
        m_horizontalAim = 0;
        m_verticalAim = 0;
        m_scroll = 0;
        m_jumpOn = false;
        m_crouching = false;
        m_enter = false;
        m_pause = false;

        m_save = false;
        m_load = false;
    }

    //Public accessors
    public float xMove
    {
        get{return m_xMove;}
    }
    public float vMove
    {
        get{return m_vMove;}
    }
    public float horizontalAim
    {
        get{return m_horizontalAim;}
    }
    public float verticalAim
    {
        get{return m_verticalAim;}
    }
    public float scroll
    {
        get{return m_scroll;}
    }
    public bool jumpOn
    {
        get{return m_jumpOn;}
    }
    public bool crouching
    {
        get{return m_crouching;}
    }
    public bool enter
    {
        get{return m_enter;}
    }
    public bool pause
    {
        get{return m_pause;}
    }
    public bool save
    {
        get{return m_save;}
    }
    public bool load
    {
        get{return m_load;}
    }

}
