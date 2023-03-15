using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "New Utensil")]
public class Utensil_ScrObj : ScriptableObject
{
    public Card_Type type = Card_Type.utensil;
    public int maxAmount;
    public int utensilID;
    public Sprite utensilSprite;
}
