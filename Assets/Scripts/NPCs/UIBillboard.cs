using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboard : MonoBehaviour {

    [SerializeField] float scaleFactor;
    [SerializeField] float maxScale;
    [SerializeField] float minScale;

    Transform camTransform;
    Quaternion originalRotation;
    Vector3 originalScale;

    void Start () {
        originalRotation = transform.rotation;
        camTransform = Camera.main.transform;
        originalScale = transform.localScale;
    }

    void Update () {
        transform.rotation = camTransform.rotation * originalRotation;
        transform.localScale = Mathf.Clamp ((camTransform.position - transform.position).magnitude * scaleFactor, minScale, maxScale) * originalScale;
    }
}
