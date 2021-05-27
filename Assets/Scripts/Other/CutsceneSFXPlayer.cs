using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneSFXPlayer : MonoBehaviour {

    public List<AudioClip> SFX = new List<AudioClip> ();

    public void PlayClip (string name) {
        int targetSoundID = 0;

        for (int i = 0; i < SFX.Count; i++) {
            if (SFX [i].name == name) {
                targetSoundID = i;
                break;
            }
        }

        AudioManager.Play2DSound (SFX [targetSoundID], Statics.SFXMixerGroupName, 1, false);
    }
}
