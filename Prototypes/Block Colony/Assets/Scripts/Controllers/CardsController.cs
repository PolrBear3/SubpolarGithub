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
    private List<Card> _handCards = new();


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

    //
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
            _handCards.Add(card);

            return;
        }
    }
}
