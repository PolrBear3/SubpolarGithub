using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractCondition
{
    public bool Interactable();
}

public class Card_Interaction : MonoBehaviour, IInteractCondition
{
    [Space(20)]
    [SerializeField] private Card _card;

    [SerializeField] private GameObject _cardPointer;
    public GameObject cardPointer => _cardPointer;
    
    
    private Card _pointingCard;
    public Card pointingCard => _pointingCard;

    private Card _interactedCard;
    public Card interactedCard => _interactedCard;


    public Action<Card> OnInteract;


    // MonoBehaviour
    private void Start()
    {
        _cardPointer.SetActive(false);
        
        // subscriptions
    }


    // IInteractCondition (use on custom card components)
    public bool Interactable()
    {
        return false;
    }


    // From Other Card
    public void Reset_InteractData()
    {
        if (_card.movement.dragging) return;

        _pointingCard = null;
        _interactedCard = null;
    }
    public void Interact_PointedCard()
    {
        if (_pointingCard == null) return;

        OnInteract?.Invoke(_pointingCard);
        _interactedCard = _pointingCard;
    }
    
    
    // Current Card
    private bool Card_Interactable(Card card)
    {
        if (!card.gameObject.TryGetComponent(out IInteractCondition interactCondition)) return false;
        return interactCondition.Interactable();
    }

    public void Point_ClosestCard()
    {
        Card_Detection detection = _card.detection;

        List<Card> detectedCards = detection.Closest_DetectedCards();
        if (detectedCards.Count == 0) return;

        bool cardPointed = false;

        for (int i = 0; i < detectedCards.Count; i++)
        {
            Card card = detectedCards[i];
            bool toggle = !cardPointed && Card_Interactable(card);

            detectedCards[i].interaction.cardPointer.SetActive(toggle);

            if (toggle == false || toggle && cardPointed) continue;

            _pointingCard = card;
            cardPointed = true;
        }
    }
    public void UpdateCards_Pointer()
    {
        List<Card> allCards = Game_Controller.instance.tableTop.currentCards;

        for (int i = 0; i < allCards.Count; i++)
        {
            if (_card.movement.dragging && allCards[i] == _pointingCard) continue;

            allCards[i].interaction.cardPointer.SetActive(false);
        }
    }
}
