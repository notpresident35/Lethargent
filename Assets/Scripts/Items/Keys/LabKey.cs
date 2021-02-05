using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LabKey : Key
{
    protected override void Awake()
    {
        name = "Lab Key";
        id = 4;
        normal = false;
        door = "Lab";
    }

}
