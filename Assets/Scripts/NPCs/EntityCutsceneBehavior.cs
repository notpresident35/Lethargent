using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityCutsceneBehaviorSO;

public class EntityCutsceneBehavior : MonoBehaviour {

    [SerializeField] bool active;

    [Header ("Behaviors")]
    [SerializeField] EntityCutsceneBehaviorSO [] behaviors;

    EntityController entity;
    int behaviorIndex;
    float iterator;
    CutsceneBehavior currentBehavior;

    private void Update () {

        if (!active) { return; }

        if (!currentBehavior.waitForContinue && iterator > currentBehavior.unskippableLength) {
            Continue ();
            iterator = 0;
        }
        iterator += Time.deltaTime;
    }

    public void Continue () {

        if (!active) { return; }

        behaviorIndex++;
        currentBehavior = behaviors [CutsceneManager.CutsceneID].behaviors [behaviorIndex];
        entity.active = currentBehavior.entityControllerActive;
        entity.SetBehaviorOverrideQueue (currentBehavior.behavior);
    }
}
