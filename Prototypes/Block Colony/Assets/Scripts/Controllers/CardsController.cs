using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsController : MonoBehaviour
{
    [Header("")]
    [SerializeField] private MainController _main;
    public MainController main => _main;

    [Header("")]
    [SerializeField] private RectTransform _spawnPoint;

    [SerializeField] private Card_SnapPoint[] _snapPoints;
    public Card_SnapPoint[] snapPoints => _snapPoints;

    private List<CardScrObj> _deckCards = new();
    private List<Card> _drawnCards = new();


    // UnityEngine
    private void Start()
    {
        _main.TestEvent += Draw_RandomCard;
    }


    // test button
    private void Draw_RandomCard()
    {
        CardScrObj randCard = _main.data.Card_ScrObj();
        DrawCard_toEmpty(randCard);
    }


    // All Card Control Functions
    private void DrawCard_toEmpty(CardScrObj cardScrObj)
    {
        for (int i = 0; i < _snapPoints.Length; i++)
        {
            if (_snapPoints[i].hasCard) continue;

            GameObject spawnCard = Instantiate(_main.data.cardPrefab, _spawnPoint);

            Card card = spawnCard.GetComponent<Card>();
            CardData cardData = new(cardScrObj);

            // new data set to drawn card
            card.Set_CurrentData(cardData);

            // data update to snappoint & move to empty slot
            _snapPoints[i].Set_CurrentCard(card);

            // controller data update
            _drawnCards.Add(card);

            return;
        }
    }

    private void Reorder_DrawnCards()
    {
        for (int i = 0; i < _snapPoints.Length; i++)
        {
            if (i > _drawnCards.Count - 1)
            {
                _snapPoints[i].Set_CurrentCard(null);
                continue;
            }

            _snapPoints[i].Set_CurrentCard(_drawnCards[i]);
        }
    }

    public void Remove_DrawnCard(Card removeCard)
    {
        _drawnCards.Remove(removeCard);
        Destroy(removeCard.gameObject);

        Reorder_DrawnCards();
    }
}
