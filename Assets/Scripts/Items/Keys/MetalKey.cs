using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MetalKey : Key
{
    protected override void Awake()
    {
        name = "Metal Key";
        id = 1;
        normal = false;
        door = "Maintenance";
    }

}
