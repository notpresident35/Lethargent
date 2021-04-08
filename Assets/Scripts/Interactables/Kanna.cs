using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kanna : GenericInteractable {

    public override void Interact () {
        CutsceneManager.Singleton.StartCutscene (2);
    }
}
