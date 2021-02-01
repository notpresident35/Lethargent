using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntityController;

// Contains all behaviors for one cutscene
[CreateAssetMenu (fileName = "New Behavior", menuName = "ScriptableObjects/EntityCutsceneBehavior", order = 1)]
public class EntityCutsceneBehaviorSO : ScriptableObject {

    [System.Serializable]
    public struct CutsceneBehavior {
        public bool entityControllerActive;
        public bool waitForContinue;
        public float unskippableLength;
        public Behavior behavior;
    }

    public CutsceneBehavior [] behaviors;
}
