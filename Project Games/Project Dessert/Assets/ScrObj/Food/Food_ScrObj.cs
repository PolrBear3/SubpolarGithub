using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "New Food")]
public class Food_ScrObj : ScriptableObject
{
    public Card_Type type = Card_Type.food;
    public int foodID;
    public Sprite foodSprite;
    public Sprite icon;
}
