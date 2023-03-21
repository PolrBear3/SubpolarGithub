using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Card_Type { food, utensil, manager }

[System.Serializable]
public struct Card_Type_Icon
{
    public Card_Type cardType;
    public Sprite typeIcon;
}

public class DataBase : MonoBehaviour
{
    public GameObject blankCard;
    public List<GameObject> defaultCards = new List<GameObject>();

    public Card_Type_Icon[] typeIconDatas;
    public Food_ScrObj[] foods;
    public Utensil_ScrObj[] utensils;

    public Sprite Find_CardType_Icon(Card_Type type)
    {
        for (int i = 0; i < typeIconDatas.Length; i++)
        {
            if (type != typeIconDatas[i].cardType) continue;
            return typeIconDatas[i].typeIcon;
        }
        return null;
    }
    public Food_ScrObj Find_Food(int id)
    {
        for (int i = 0; i < foods.Length; i++)
        {
            if (id != foods[i].foodID) continue;
            return foods[i];
        }
        return null;
    }
}