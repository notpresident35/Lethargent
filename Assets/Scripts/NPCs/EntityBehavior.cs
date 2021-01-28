using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    bool behaviorDelayed;

    EntityController entity;

    // TODO: Called by OnGameStart
    [ContextMenu ("Start game")]
    void SetupBehavior () {

        entity = GetComponent<EntityController> ();
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
        if (behaviors [currentBehaviorIndex + 1]._time < TimeSystem.currentTime) {
            currentBehaviorIndex++;
            entity.SetBehavior (behaviors [currentBehaviorIndex]._behavior);
        }
    }
}
