using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assignment : GenericInteractable {

    public override void Interact () {
        CutsceneManager.Singleton.StartCutscene (4);
    }
}
