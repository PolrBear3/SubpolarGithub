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
        OnInteract += Stack_PointedCard;
    }

    private void OnDestroy()
    {
        // subscriptions
        OnInteract -= Stack_PointedCard;
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
            detectedCards[i].interaction.Toggle_Pointer(detectedCards[i] == closestCard);
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
    
    
    // Interactions
    public void Interact_PointedCard()
    {
        List<Card> detectedCards = _card.detection.detectedCards;
        if (detectedCards.Count == 0) return;
        
        for (int i = 0; i < detectedCards.Count; i++)
        {
            if (!detectedCards[i].interaction.pointerToggled) continue;
            
            OnInteract?.Invoke(detectedCards[i]);
            return;
        }
    }

    
    private void CheckInteract_Debug(Card pointedCard)
    {
        List<Card> detectedCards = _card.detection.detectedCards;

        for (int i = 0; i < detectedCards.Count; i++)
        {
            if (pointedCard != detectedCards[i]) continue;
            
            Debug.Log("Interact Card Index Number: " + i);
            return;
        }
    }

    private void Stack_PointedCard(Card pointedCard)
    {
        Card_Data currentCardData = _card.data;
        Card_Data pointedCardData = pointedCard.data;
        
        if (_card.data.cardScrObj != pointedCardData.cardScrObj) return;
        
        int setAmount = currentCardData.stackAmount + pointedCardData.stackAmount;
        currentCardData.Set_StackAmount(setAmount);

        Game_Controller.instance.tableTop.currentCards.Remove(pointedCard);
        Destroy(pointedCard.gameObject);
    }
}
