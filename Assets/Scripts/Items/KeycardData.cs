using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewItem", menuName = "ScriptableObjects/KeycardData")]
public class KeycardData : ItemData {
    
    [System.Serializable]
    public enum Color {
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
        Black
    }

    public Color color;
}
