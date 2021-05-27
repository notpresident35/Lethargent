using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assignment : GenericInteractable {

    public GameObject Gateway;
    public List<MeshRenderer> Papers;
    public Material PaperMat;

    public override void Interact () {
        //CutsceneManager.Singleton.StartCutscene (4);
        Gateway.SetActive (false);
        foreach (MeshRenderer paper in Papers) {
            paper.material = PaperMat;
            paper.GetComponent<Outline> ().enabled = false;
        }
        FindObjectOfType<PlayerMechanics> ().RemoveHeldItem ();
        transform.GetChild (0).gameObject.SetActive (false);
    }
}
