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
    
    
    private bool _pointerToggled;
    public bool pointerToggled => _pointerToggled;

    private bool _interacted;
    public bool interacted => _interacted;

    public Action<Card> OnInteract;


    // MonoBehaviour
    private void Start()
    {
        Toggle_Pointer(false);
        
        // subscriptions
    }


    // IInteractCondition (use on custom card components)
    public bool Interactable()
    {
        return false;
    }


    // From Other Card
    public void Toggle_Pointer(bool toggle)
    {
        _pointerToggled = toggle;
        _cardPointer.SetActive(_pointerToggled);
    }

    public void ResetFlag_Interacted()
    {
        if (_card.movement.dragging) return;

        _interacted = false;
    }
    public void Interact_PointedCard()
    {
        List<Card> detectedCards = _card.detection.detectedCards;

        for (int i = 0; i < detectedCards.Count; i++)
        {
            if (detectedCards[i] == null) continue;
            if (detectedCards[i].interaction.pointerToggled == false) continue;
            
            OnInteract?.Invoke(detectedCards[i]);
            _interacted = true;
        }
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
            if (card == null) continue;

            bool toggle = !cardPointed && Card_Interactable(card);
            detectedCards[i].interaction.Toggle_Pointer(toggle);

            cardPointed = toggle;
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
}
