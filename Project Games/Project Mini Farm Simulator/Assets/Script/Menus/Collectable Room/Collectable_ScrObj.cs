using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Collectable_Rare_level { wood, silver, gold, diamond }

[CreateAssetMenu(menuName = "New Collectable")]
public class Collectable_ScrObj : ScriptableObject
{
    public int collectableID;
    public Sprite sprite;
    public Collectable_Rare_level rareLevel;
}
