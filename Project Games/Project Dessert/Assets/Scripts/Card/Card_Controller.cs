using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Card_Data
{
    public Card_Type type;
    public int currentAmount;
    public Food_ScrObj food;
    public Utensil_ScrObj utensil;
}

public class Card_Controller : MonoBehaviour
{
    [HideInInspector] public  Game_Controller controller;

    [HideInInspector] public DragDrop_System dragDrop;
    [HideInInspector] public CardDetection_System detection;

    public Card_Data data;

    public SpriteRenderer icon;
    public TextMesh amountText;
    public SpriteRenderer main;

    private void Awake()
    {
        dragDrop = gameObject.GetComponent<DragDrop_System>();
        detection = gameObject.GetComponent<CardDetection_System>();
    }

    public void New_Card(Game_Controller controller, Food_ScrObj food, Utensil_ScrObj utensil)
    {
        this.controller = controller;

        if (food != null)
        {
            data.food = food;
            main.sprite = data.food.foodSprite;
            data.type = Card_Type.food;
        }
        else if (utensil != null)
        {
            data.utensil = utensil;
            main.sprite = data.utensil.utensilSprite;
            data.type = Card_Type.utensil;
        }

        icon.sprite = controller.dataBase.Find_CardType_Icon(data.type);
        data.currentAmount = 1;
        amountText.text = data.currentAmount.ToString();
    }

    public bool Combine_Check(Card_Controller cardController)
    {
        if (cardController.data.type == Card_Type.food)
        {
            if (cardController.data.food != data.food) return false;
            // dragging card max check
            if (cardController.data.currentAmount == data.food.maxAmount) return false;
            // dropped cards max check
            if (data.currentAmount == data.food.maxAmount) return false;
            return true;
        }
        else if (cardController.data.type == Card_Type.utensil)
        {
            if (cardController.data.utensil != data.utensil) return false;
            // dragging card max check
            if (cardController.data.currentAmount == data.utensil.maxAmount) return false;
            // dropped cards max check
            if (data.currentAmount == data.utensil.maxAmount) return false;
            return true;
        }

        return false;
    }

    public void Combine_Amount()
    {

    }
    public void Split_Amount() 
    {
        data.currentAmount -= 1;
        amountText.text = data.currentAmount.ToString();
    }
}