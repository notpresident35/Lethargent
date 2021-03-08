using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpUI : MonoBehaviour {

    [SerializeField] float HelpDelay = 2;
    [SerializeField] float AppearSpeed = 2;
    [SerializeField] Transform HelpBackground;
    [SerializeField] Text HelpText;

    float helpIterator = 0;
    PlayerCollisions collisions;
    InputManager control;
    bool hasInteracted;
    Color textCol;
    Vector3 backgroundScale;

    private void Awake () {
        backgroundScale = HelpBackground.localScale;
        collisions = FindObjectOfType<PlayerCollisions> ();
        control = FindObjectOfType<InputManager> ();
        textCol = HelpText.color;
        textCol.a = 0;
        HelpText.color = textCol;
        HelpBackground.localScale = Vector3.zero;
    }

    // TODO: Make this script only check physics every X frames for performance
    private void Update () {
        if (collisions.CheckInteract ()) {
            if (hasInteracted) {
                helpIterator = 0;
                textCol.a = Mathf.Clamp01 (textCol.a - Time.deltaTime * AppearSpeed);
            } else {
                helpIterator += Time.deltaTime;
                if (helpIterator > HelpDelay) {
                    textCol.a = Mathf.Clamp01 (textCol.a + Time.deltaTime * AppearSpeed);
                }
            }
        } else {
            helpIterator = 0;
            hasInteracted = false;
            textCol.a = Mathf.Clamp01 (textCol.a - Time.deltaTime * AppearSpeed);
        }

        if (control.interact) {
            helpIterator = 0;
            hasInteracted = true;
        }

        HelpText.color = textCol;
        HelpBackground.localScale = textCol.a * backgroundScale;
    }
}
