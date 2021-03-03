using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour {

    [SerializeField] bool useAmbientLights = false;
    [SerializeField] Gradient SkyColor;
    [SerializeField] Gradient EquatorColor;
    [SerializeField] Gradient GroundColor;
    [SerializeField] AnimationCurve AmbientLightStrength;
    [SerializeField] float debugTimeValue;
    [SerializeField] Light [] AmbientLights;

    private static int ambientSkyColorID = Shader.PropertyToID ("ambient_skycolor");
    private static int ambientEquatorColorID = Shader.PropertyToID ("ambient_equatorcolor");
    private static int ambientGroundColorID = Shader.PropertyToID ("ambient_groundcolor");

    private void Update () {
        ApplyLightingAtTime (TimeSystem.CurrentTime % Statics.DayLength);
    }

    // Finds the nearest entry before the current time, then blends that entry with the next entry based on the current time
    void ApplyLightingAtTime (float time) {
        RenderSettings.ambientSkyColor = SkyColor.Evaluate (time);
        RenderSettings.ambientEquatorColor = EquatorColor.Evaluate (time);
        RenderSettings.ambientGroundColor = GroundColor.Evaluate (time);

        if (useAmbientLights) {
            for (int i = 0; i < AmbientLights.Length; i++) {
                AmbientLights [i].intensity = AmbientLightStrength.Evaluate (time);
            }
            AmbientLights [0].gameObject.SetActive (true);
        } else {
            AmbientLights [0].gameObject.SetActive (false);
        }

        // TODO: Measure the performance of this! Depending on how this is handled internally, this could KILL performance on some machines
        Shader.SetGlobalColor (ambientSkyColorID, RenderSettings.ambientSkyColor);
        Shader.SetGlobalColor (ambientEquatorColorID, RenderSettings.ambientEquatorColor);
        Shader.SetGlobalColor (ambientGroundColorID, RenderSettings.ambientGroundColor);
    }

    [ContextMenu ("Apply debug lighting")]
    void ApplyDebugLghting () {
        RenderSettings.ambientSkyColor = SkyColor.Evaluate (debugTimeValue);
        RenderSettings.ambientEquatorColor = EquatorColor.Evaluate (debugTimeValue);
        RenderSettings.ambientGroundColor = GroundColor.Evaluate (debugTimeValue);

        if (useAmbientLights) {
            for (int i = 0; i < AmbientLights.Length; i++) {
                AmbientLights [i].intensity = AmbientLightStrength.Evaluate (debugTimeValue);
            }
            AmbientLights [0].gameObject.SetActive (true);
        } else {
            AmbientLights [0].gameObject.SetActive (false);
        }

        // TODO: Measure the performance of this! Depending on how this is handled internally, this could KILL performance on some machines
        Shader.SetGlobalColor (ambientSkyColorID, RenderSettings.ambientSkyColor);
        Shader.SetGlobalColor (ambientEquatorColorID, RenderSettings.ambientEquatorColor);
        Shader.SetGlobalColor (ambientGroundColorID, RenderSettings.ambientGroundColor);
    }

    [ContextMenu ("Apply ambient colors")]
    void ApplyAmbientColors () {
        Shader.SetGlobalColor (ambientSkyColorID, RenderSettings.ambientSkyColor);
        Shader.SetGlobalColor (ambientEquatorColorID, RenderSettings.ambientEquatorColor);
        Shader.SetGlobalColor (ambientGroundColorID, RenderSettings.ambientGroundColor);
    }
}
