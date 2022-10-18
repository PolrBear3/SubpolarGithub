using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Collectable_Rare_level { red, orange, yellow, green, blue, purple }

[CreateAssetMenu(menuName = "New Collectable")]
public class Collectable_ScrObj : ScriptableObject
{
    public Sprite sprite;
    public Collectable_Rare_level colorLevel;
}
