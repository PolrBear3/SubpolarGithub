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
    }

    private void OnDestroy()
    {
        // subscriptions
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

    
    public void Stack_Card(Card stackCard)
    {
        if (stackCard == null) return;
        _card.data.stackedCardDatas.Add(stackCard.data);
        
        Game_Controller.instance.tableTop.currentCards.Remove(stackCard);
        Destroy(stackCard.gameObject);
    }

    public void Spawn_StackedCard()
    {
        List<Card_Data> stackedCardDatas = _card.data.stackedCardDatas;
        if (stackedCardDatas.Count == 0) return;

        Card_Data recentCardData = stackedCardDatas[stackedCardDatas.Count - 1];
        
        Vector2 spawnPos = _card.RandomPeripheral_SpawnPosition();
        _card.cardLauncher.Launch_Card(spawnPos).Set_Data(recentCardData);

        stackedCardDatas.Remove(recentCardData);
    }
    
    
    // Examples for Custom Cards (OnInteract Subscriptions)
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
        Stack_Card(pointedCard);
    }

    public void Spawn_StackedCard_OnEmpty()
    {
        if (_card.movement.dragging) return;
        if (_card.detection.detectedCards.Count > 0) return;
        
        Spawn_StackedCard();
    }
}
