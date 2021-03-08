using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecticalBox : GenericInteractable {

    public Material mat1;
    public Material mat2;

    public override void Interact () {
        //Debug.Log (gameObject.name + " has been interacted with!");
        GetComponent<MeshRenderer> ().material = mat2;
        StartCoroutine (resetMat ());
    }

    IEnumerator resetMat () {
        yield return new WaitForSeconds (4);
        GetComponent<MeshRenderer> ().material = mat1;
    }
}
