using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Interaction : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Card _card;

    [SerializeField] private GameObject _cardPointer;
    public GameObject cardPointer => _cardPointer;
    
    
    private bool _pointerToggled;
    public bool pointerToggled => _pointerToggled;

    public Action<Card> OnInteract;


    // MonoBehaviour
    private void Start()
    {
        Toggle_Pointer(false);
        
        // subscriptions
        // OnInteract += Stack_PointedCard;
    }

    private void OnDestroy()
    {
        // subscriptions
        // OnInteract -= Stack_PointedCard;
    }


    // From Other Card
    public void Toggle_Pointer(bool toggle)
    {
        _pointerToggled = toggle;
        _cardPointer.SetActive(_pointerToggled);
    }
    
    
    // Current Card
    public void Point_ClosestCard()
    {
        Card_Detection detection = _card.detection;
        List<Card> detectedCards = detection.detectedCards;
        
        if (detectedCards.Count == 0) return;
        Card closestCard = detection.Closest_DetectedCard();

        for (int i = 0; i < detectedCards.Count; i++)
        {
            if (detectedCards[i] == null) continue;

            bool toggle = closestCard != null && detectedCards[i] == closestCard;
            detectedCards[i].interaction.Toggle_Pointer(toggle);
        }
    }

    public void UpdateCards_Pointer()
    {
        List<Card> allCards = Game_Controller.instance.tableTop.currentCards;
        List<Card> detectedCards = _card.detection.detectedCards;

        for (int i = 0; i < allCards.Count; i++)
        {
            if (_card.movement.dragging && detectedCards.Contains(allCards[i])) continue;
            allCards[i].interaction.Toggle_Pointer(false);
        }
    }
    
    
    // Select
    public void Interact_PointedCard()
    {
        List<Card> detectedCards = _card.detection.detectedCards;

        for (int i = 0; i < detectedCards.Count; i++)
        {
            if (detectedCards[i] == null) continue;
            if (detectedCards[i].interaction.pointerToggled == false) continue;
            
            OnInteract?.Invoke(detectedCards[i]);
        }
    }
    
    private void Stack_PointedCard(Card pointedCard)
    {
        Card_Data currentCardData = _card.data;
        Card_Data pointedCardData = pointedCard.data;
        
        if (_card.data.cardScrObj != pointedCardData.cardScrObj) return;
        
        int setAmount = currentCardData.stackAmount + pointedCardData.stackAmount;
        currentCardData.Set_StackAmount(setAmount);

        pointedCard.detection.collider.enabled = false;
        
        Game_Controller.instance.tableTop.currentCards.Remove(pointedCard);
        Destroy(pointedCard.gameObject);
    }
    
    
    // Multi Select
    public void Drop_StackedCard()
    {
        Card_Movement cardMovement = _card.movement;
        
        Card_Data stackedCardData = _card.data;
        int stackedAmount = stackedCardData.stackAmount;
        
        if (cardMovement.dragging == false || stackedAmount <= 1) return;
        
        Game_Controller gameController = Game_Controller.instance;

        Vector2 spawnPos = _card.RandomPeripheral_SpawnPosition();
        Card launchedCard = _card.cardLauncher.Launch_Card(spawnPos);
        
        launchedCard.transform.SetParent(gameController.tableTop.allCards);

        launchedCard.Set_Data(new(stackedCardData.cardScrObj));
        launchedCard.Update_Visuals();

        stackedCardData.Set_StackAmount(stackedAmount - 1);
        gameController.cursor.Update_HoverCardInfo(_card);
    }
}
