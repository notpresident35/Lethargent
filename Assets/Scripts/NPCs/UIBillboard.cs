using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboard : MonoBehaviour {

    [SerializeField] float scaleFactor;
    [SerializeField] bool useScaling = true;
    [SerializeField] float maxScale;
    [SerializeField] float minScale;
    [SerializeField] float maxScaleRange;
    [SerializeField] float easingSpeed;
    [SerializeField] AnimationCurve easingCurve;

    Transform camTransform;
    Quaternion originalRotation;
    Vector3 originalScale;
    float easing;

    void Start () {
        originalRotation = transform.rotation;
        camTransform = Camera.main.transform;
        originalScale = transform.localScale;
    }

    void Update () {
        transform.rotation = camTransform.rotation/* * originalRotation*/;
        if (useScaling) {
            float scale = (camTransform.position - transform.position).magnitude * scaleFactor;
            if (scale < maxScale * maxScaleRange) {
                easing = Mathf.Min (easing + Time.deltaTime * easingSpeed, 1);
            } else {
                easing = Mathf.Max (easing - Time.deltaTime * easingSpeed, 0);
            }
            transform.localScale = Mathf.Clamp (scale, minScale, maxScale) * originalScale * easingCurve.Evaluate (easing);
        }
    }
}
