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
        }

        _currentCard = setCard;
        _hasCard = true;

        // move card to current snappoint (add animation)
        setCard.transform.SetParent(transform);
        setCard.transform.localPosition = Vector2.zero;
    }
}
