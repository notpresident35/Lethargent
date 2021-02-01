using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityCutsceneBehaviorSO;

public class EntityCutsceneBehavior : MonoBehaviour {

    [SerializeField] bool active;

    [Header ("Behaviors")]
    // One entry for every cutscene (must be ordered correctly)
    [SerializeField] EntityCutsceneBehaviorSO [] behaviors;

    EntityController entity;
    int behaviorIndex;
    float iterator;
    CutsceneBehavior currentBehavior;

    private void OnEnable () {
        CutsceneManager.CutsceneContinue += Continue;
    }

    private void OnDisable () {
        CutsceneManager.CutsceneContinue -= Continue;
    }

    // This runs parallel to the CutsceneContinue event because some cutscenes will have shots where the NPC does multiple things in one shot
    private void Update () {

        if (!active) { iterator = 0; return; }

        if (!currentBehavior.waitForContinue && iterator > currentBehavior.unskippableLength) {
            Continue ();
            iterator = 0;
        }
        iterator += Time.deltaTime;
    }

    public void StartCutscene () {
        active = true;
        behaviorIndex = 0;
        currentBehavior = behaviors [CutsceneManager.CutsceneID].behaviors [0];
        entity.active = currentBehavior.entityControllerActive;
        entity.SetBehaviorOverrideQueue (currentBehavior.behavior);
    }

    public void Continue () {

        if (!active) { return; }

        behaviorIndex++;
        currentBehavior = behaviors [CutsceneManager.CutsceneID].behaviors [behaviorIndex];
        entity.active = currentBehavior.entityControllerActive;
        entity.SetBehaviorOverrideQueue (currentBehavior.behavior);
    }

    public void EndCutscene () {
        active = false;
        entity.active = currentBehavior.entityControllerActive;
    }
}
