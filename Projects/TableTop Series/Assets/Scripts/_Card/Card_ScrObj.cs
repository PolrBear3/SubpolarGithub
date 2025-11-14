using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ New Card")]
public class Card_ScrObj : ScriptableObject
{
    [Space(20)]
    [SerializeField] private string _cardName;
    public string cardName => _cardName;

    [SerializeField] private Sprite _iconSprite;
    public Sprite iconSprite => _iconSprite;

    [SerializeField] private Sprite _baseSprite;
    public Sprite baseSprite => _baseSprite;

    [Space(20)]
    [SerializeField] private Card_IngredientData[] _ingredientDatas;
    public Card_IngredientData[] ingredientDatas => _ingredientDatas;
}
