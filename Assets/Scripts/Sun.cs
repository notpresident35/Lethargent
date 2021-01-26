using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {

    Vector3 startRotation;

    void Start () {
        startRotation = transform.rotation.eulerAngles;
    }

    void Update () {
        transform.rotation = Quaternion.Euler (TimeSystem.currentTime * 360f / Statics.DayLength, startRotation.y, startRotation.z);
    }
}
