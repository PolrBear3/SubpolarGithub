using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Card_Data
{
    public int currentAmount;
}

public class Card_Controller : MonoBehaviour
{
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

    public void Update_Card(Sprite main, Sprite icon)
    {
        this.main.sprite = main;
        this.icon.sprite = icon;
        data.currentAmount = 1;
        amountText.text = data.currentAmount.ToString();
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