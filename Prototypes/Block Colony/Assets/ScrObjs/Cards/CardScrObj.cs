using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Card")]
public class CardScrObj : ScriptableObject
{
    public int id;
    public string cardName;

    public Sprite cardSprite;
    public Sprite iconSprite;

    [TextArea(3, 10)]
    public string description;

    public GameObject cardPrefab;
}
