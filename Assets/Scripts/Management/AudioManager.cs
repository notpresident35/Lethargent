using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public static AudioManager Singleton;
    public static Queue<AudioSource> sourcePool = new Queue<AudioSource> ();
    [Range (0, 1)]
    public static float AmbienceVolume = 1f;
    [Range (0, 1)]
    public static float SFXVolume = 1f;

    private void Awake () {
        if (!Singleton) {
            Singleton = this;
            DontDestroyOnLoad (gameObject);
        } else {
            Destroy (gameObject);
        }

        AmbienceVolume = PlayerPrefs.GetFloat (Statics.AmbienceVolumePlayerPrefName, 1);
        SFXVolume = PlayerPrefs.GetFloat (Statics.SFXVolumePlayerPrefName, 1);
    }

    // Uses object pooling to play a sound effect in 2D space
    // Whether the sound is SFX or ambience is determined by its mixerGroup
    public static void Play2DSound (AudioClip clip, AudioMixerGroup mixerGroup, float volume, bool ignoreGamePause) {
        AudioSource source;
        if (sourcePool.Count > 0) {
            source = sourcePool.Dequeue ();
            source.gameObject.SetActive (true);
        } else {
            source = new GameObject ("Audio Source", typeof (AudioSource)).GetComponent<AudioSource> ();
            source.transform.parent = Singleton.transform;
        }
        source.clip = clip;
        source.spatialBlend = 0;
        source.outputAudioMixerGroup = mixerGroup;
        // SFXVolume controls the mixer volume, and is thus ignored here
        source.volume = volume;
        source.ignoreListenerPause = ignoreGamePause;
        source.Play ();
        Singleton.StartCoroutine (EnqueueSource (source));
    }

    // Uses object pooling to play a sound effect at a position in 3D space
    // Whether the sound is SFX or ambience is determined by its mixerGroup
    // Do NOT use this for player gunfire, or it will sound strange if the player moves side to side while shooting
    public static void Play3DSound (AudioClip clip, AudioMixerGroup mixerGroup, Vector3 position, float volume, bool ignoreGamePause) {
        AudioSource source;
        if (sourcePool.Count > 0) {
            source = sourcePool.Dequeue ();
            source.gameObject.SetActive (true);
        } else {
            source = new GameObject ("Audio Source", typeof (AudioSource)).GetComponent<AudioSource> ();
            source.transform.parent = Singleton.transform;
        }
        source.clip = clip;
        source.spatialBlend = 1;
        source.maxDistance = Statics.SFXRange;
        source.transform.position = position;
        source.outputAudioMixerGroup = mixerGroup;
        // SFXVolume controls the mixer volume, and is thus ignored here
        source.volume = volume;
        source.ignoreListenerPause = ignoreGamePause;
        source.Play ();
        Singleton.StartCoroutine (EnqueueSource (source));
    }

    // Returns the audio source to the object pool once it finishes playing
    public static IEnumerator EnqueueSource (AudioSource source) {
        yield return new WaitForSeconds (source.clip.length);
        source.Stop ();
        source.gameObject.SetActive (false);
        sourcePool.Enqueue (source);
    }

    // Useful for things like UI Sliders in a settings menu
    public void SetSFXVolume (float volume) {
        SFXVolume = volume;
    }
    public void SetAmbienceVolume (float volume) {
        AmbienceVolume = volume;
    }
}
