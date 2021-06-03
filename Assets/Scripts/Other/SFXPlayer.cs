using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour {

    public AudioClip StepSFX;
    public float volume;
    public bool spatial = true;
    //public AudioClip JumpSFX;

    public void PlayStepSFX () {
        if (!spatial) {
            AudioManager.Play2DSound (StepSFX, Statics.SFXMixerGroupName, volume, false, Random.Range (0.9f, 1.1f));
        } else {
            AudioManager.Play3DSound (StepSFX, Statics.SFXMixerGroupName, transform.position, volume, false, Random.Range (0.9f, 1.1f));
        }
    }
/*
    public void PlayJumpSFX () {

    }*/
}
