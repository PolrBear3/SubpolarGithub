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
    [HideInInspector] public Game_Controller controller;

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
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.A)) return;
        Spawn_OtherCards(Card_Type.food, 0, 3);
    }

    // card update
    public void Update_Card(Game_Controller controller, Card_Type cardType, int id)
    {
        this.controller = controller;

        if (Card_Type.food == cardType)
        {
            data.food = controller.dataBase.Find_Food(id);
            main.sprite = data.food.foodSprite;
            data.type = Card_Type.food;
        }
        else if (Card_Type.utensil == cardType)
        {
            data.utensil = controller.dataBase.Find_Utensil(id);
            main.sprite = data.utensil.utensilSprite;
            data.type = Card_Type.utensil;
        }
        else if (Card_Type.manager == cardType)
        {
            data.type = Card_Type.manager;
        }

        icon.sprite = controller.dataBase.Find_CardType_Icon(data.type);
        data.currentAmount = 1;
        amountText.text = data.currentAmount.ToString();
    }

    // other cards spawn function
    public void Spawn_OtherCards(Card_Type cardType, int id, int amount)
    {
        StartCoroutine(Spawn_OtherCards_Delay(cardType, id, amount));
    }
    private IEnumerator Spawn_OtherCards_Delay(Card_Type cardType, int id, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // set spawn point random range -1 to 1
            float xRange = Random.Range(-1f, 1f);
            float yRange = Random.Range(-1f, 1f);

            // spawn card
            var spawnedCard = Instantiate(controller.dataBase.blankCard, transform.position, Quaternion.identity);
            spawnedCard.transform.parent = transform;
            spawnedCard.transform.localPosition = new Vector2(xRange, yRange);
            
            // move spawn card to field cards point
            spawnedCard.transform.parent = controller.trackSystem.transform;

            // get card controller data
            if (!spawnedCard.TryGetComponent(out Card_Controller cardController)) continue;

            // update card and add to track system
            cardController.Update_Card(controller, cardType, id);
            controller.trackSystem.Addto_Track(cardController);

            yield return new WaitForSeconds(1f);
        }
    }

    // card check system
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
        else if (cardController.data.type == Card_Type.manager)
        {
            // manager card check
            return true;
        }

        return false;
    }

    // card data system
    public void Increase_Amount(int amount)
    {
        data.currentAmount += amount;
        amountText.text = data.currentAmount.ToString();
    }
    public void Decrease_Amount(int amount) 
    {
        data.currentAmount -= amount;
        amountText.text = data.currentAmount.ToString();
    }
}