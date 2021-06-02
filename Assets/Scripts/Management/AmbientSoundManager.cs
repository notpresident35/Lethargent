using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundManager : MonoBehaviour {
    
    [System.Serializable]
    public class AmbientSound {
        public AudioSource source;
        public Transform sourcePosition;
        public float falloffRate;
        public float range;
        [HideInInspector]
        public float volume;
    }

    public List<AmbientSound> AmbientSounds = new List<AmbientSound> ();
    public Transform AmbientListener;
    public AudioSource defaultAmbientSource;

    float defaultAmbienceVolume;

    private void Awake () {
        for (int i = 0; i < AmbientSounds.Count; i++) {
            AmbientSounds [i].volume = AmbientSounds [i].source.volume;
        }
    }

    private void Update () {

        defaultAmbienceVolume = 1;

        foreach (AmbientSound ambience in AmbientSounds) {

            float dist = Vector3.Distance (ambience.sourcePosition.position, AmbientListener.position);
            float fader = Mathf.Clamp01 ((ambience.range - dist) / ambience.range * ambience.falloffRate);
            ambience.source.volume = ambience.volume * fader;

            defaultAmbienceVolume -= ambience.source.volume;
        }

        defaultAmbientSource.volume = defaultAmbienceVolume;
    }
}
