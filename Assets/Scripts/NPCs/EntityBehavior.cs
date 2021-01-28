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

    private void Awake () {
        entity = GetComponent<EntityController> ();
    }

    // TODO: Called by OnGameStart
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

        entity.SetBehavior (behaviors [currentBehaviorIndex]._behavior);
    }

    private void Start () {
        SetupBehavior ();
    }

    private void Update () {

        if (!active) { return; }

        // If the next state is due and the current state is interruptible, switch to the next state
        if (currentBehaviorIndex < behaviors.Length - 1 && behaviors [currentBehaviorIndex]._behavior._interruptibleByNewBehavior && TimeSystem.currentTime > behaviors [currentBehaviorIndex + 1]._time) {
            currentBehaviorIndex++;
            entity.SetBehavior (behaviors [currentBehaviorIndex]._behavior);
        }
    }
}
