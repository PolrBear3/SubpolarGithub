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
    
    
    // Current Card
    public void Point_ClosestCard()
    {
        Card_Detection detection = _card.detection;
        List<Card> detectedCards = detection.detectedCards;
        
        if (detectedCards.Count == 0) return;
        Card closestCard = detection.Closest_DetectedCards()[0];

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
    
    
    // Multi Pickup
}
