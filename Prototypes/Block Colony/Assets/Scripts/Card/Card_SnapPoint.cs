using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_SnapPoint : MonoBehaviour
{
    private Card _currentCard;
    public Card currentCard => _currentCard;

    private bool _hasCard;
    public bool hasCard => _hasCard;


    //
    public void Set_CurrentCard(Card setCard)
    {
        if (setCard == null)
        {
            _currentCard = null;
            _hasCard = false;

            return;
        }

        _currentCard = setCard;
        _hasCard = true;

        CardsController controller = setCard.main.cards;

        // move card to current snappoint
        setCard.transform.SetParent(transform);
        LeanTween.moveLocal(setCard.gameObject, Vector2.zero, controller.movementTime).setEase(controller.moveType);
    }
}
