using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpUI : MonoBehaviour {

    [SerializeField] float AppearSpeed = 2;
    [SerializeField] Transform HelpBackground;
    [SerializeField] Text HelpText;
    [SerializeField] Text SuggestionText;
    [SerializeField] bool obstacleChecker;

    PlayerCollisions collisions;
    InputManager control;
    bool hasInteracted;
    Color textCol;
    Color textCol2;
    Vector3 backgroundScale;

    private void Awake () {
        backgroundScale = HelpBackground.localScale;
        collisions = FindObjectOfType<PlayerCollisions> ();
        control = FindObjectOfType<InputManager> ();
        textCol = HelpText.color;
        textCol2 = SuggestionText.color;
        textCol.a = 0;
        textCol2.a = 0;
        HelpText.color = textCol;
        HelpBackground.localScale = Vector3.zero;
    }

    // TODO: Make this script only check physics every X frames for performance
    private void Update () {
        if (obstacleChecker) {
            if (collisions.CheckGateway ()) {
                SuggestionText.text = collisions.GetGatewayText ();
                textCol2.a = Mathf.Clamp01 (textCol2.a + Time.deltaTime * AppearSpeed);
            } else {
                textCol2.a = Mathf.Clamp01 (textCol2.a - Time.deltaTime * AppearSpeed);
            }
        } else {
            if (collisions.CheckInteract ()) {
                if (hasInteracted) {
                    textCol.a = Mathf.Clamp01 (textCol.a - Time.deltaTime * AppearSpeed);
                } else {
                    textCol.a = Mathf.Clamp01 (textCol.a + Time.deltaTime * AppearSpeed);
                }
            } else {
                hasInteracted = false;
                textCol.a = Mathf.Clamp01 (textCol.a - Time.deltaTime * AppearSpeed);
            }

            if (control.interact) {
                hasInteracted = true;
            }
        }

        HelpText.color = textCol;
        HelpBackground.localScale = textCol.a * backgroundScale;
        SuggestionText.color = textCol2;
    }
}
