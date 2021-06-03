using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour {

    public AudioClip StepSFX;
    //public AudioClip JumpSFX;

    public void PlayStepSFX () {
        AudioManager.Play2DSound (StepSFX, Statics.SFXMixerGroupName, 1, false, Random.Range (0.9f, 1.1f));
    }
/*
    public void PlayJumpSFX () {

    }*/
}
