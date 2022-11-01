using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Collectable_Rare_level { normal, rare, special, epic, all }

[CreateAssetMenu(menuName = "New Collectable")]
public class Collectable_ScrObj : ScriptableObject
{
    public Sprite sprite;
    public Sprite lockedSprite;
    public Collectable_Rare_level colorLevel;
}
