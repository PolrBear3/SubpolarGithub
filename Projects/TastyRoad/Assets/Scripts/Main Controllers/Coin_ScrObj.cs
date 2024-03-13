using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Coin Type!")]
public class Coin_ScrObj : ScriptableObject
{
    public Sprite sprite;
    public AnimatorOverrideController spinAnim;
}
