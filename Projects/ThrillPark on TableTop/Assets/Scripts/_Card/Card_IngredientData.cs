using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card_IngredientData
{
    [SerializeField] private Card_ScrObj _ingredientCard;
    public Card_ScrObj ingredientCard => _ingredientCard;

    [SerializeField][Range(1, 10)] private int _amount;
    public int amount => _amount;


    // New
    public Card_IngredientData(Card_ScrObj ingredientCard, int amount)
    {
        _ingredientCard = ingredientCard;
        _amount = amount;
    }
}