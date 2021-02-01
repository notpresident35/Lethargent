using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityController;

[RequireComponent (typeof (EntityController))]
public class EntityBehavior : MonoBehaviour {

    [System.Serializable]
    public struct TimedBehavior {
        public float _time;
        public Behavior _behavior;
    }

    [SerializeField] bool active;

    [Header ("State")]
    [SerializeField] int currentBehaviorIndex;
    [SerializeField] TimedBehavior[] behaviors;

    EntityController entity;
    bool wasActiveBeforeCutscene;

    private void Awake () {
        entity = GetComponent<EntityController> ();
    }

    // TODO: Call this from OnGameStart, not Start
    [ContextMenu ("Start game")]
    void SetupBehavior () {
        currentBehaviorIndex = 0;

        for (int i = 0; i < behaviors.Length; i++) {
            if (behaviors[i]._time < TimeSystem.currentTime) {
                currentBehaviorIndex = i;
            } else {
                break;
            }
        }

        active = true;

        entity.active = true;
        entity.SetBehavior (behaviors [currentBehaviorIndex]._behavior);
    }

    private void Start () {
        SetupBehavior ();
    }

    private void Update () {

        if (!active) { return; }

        // If the next state is due and the current state is interruptible, switch to the next state
        if (currentBehaviorIndex < behaviors.Length - 1 && TimeSystem.currentTime > behaviors [currentBehaviorIndex + 1]._time) {
            currentBehaviorIndex++;
            entity.SetBehavior (behaviors [currentBehaviorIndex]._behavior);
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
        wasActiveBeforeCutscene = active;
        active = false;
    }

    public void StopCutscene () {
        active = wasActiveBeforeCutscene;
        if (active) {
            entity.SetBehavior (behaviors [currentBehaviorIndex]._behavior);
        }
    }
}
