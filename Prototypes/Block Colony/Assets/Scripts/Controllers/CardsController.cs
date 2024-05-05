using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Header("")]
    [SerializeField] private TextMeshProUGUI _deckCountText;


    // UnityEngine
    private void Start()
    {
        DeckCount_TextUpdate();
    }


    // Deck Control
    public void AddCard_toDeck()
    {
        _deckCards.Add(_main.data.Card_ScrObj());

        DeckCount_TextUpdate();
    }
    public void AddCard_toDeck(CardScrObj addCard)
    {
        _deckCards.Add(addCard);

        DeckCount_TextUpdate();
    }

    private void DeckCount_TextUpdate()
    {
        _deckCountText.text = _deckCards.Count.ToString();
    }


    // Draw Card Control
    public void DrawCard_fromDeck()
    {
        if (_deckCards.Count <= 0) return;

        for (int i = 0; i < _snapPoints.Length; i++)
        {
            if (_snapPoints[i].hasCard) continue;

            GameObject spawnCard = Instantiate(_main.data.cardPrefab, _spawnPoint);

            Card card = spawnCard.GetComponent<Card>();
            CardData cardData = new(_deckCards[_deckCards.Count - 1]);

            // new data set to drawn card
            card.Set_CurrentData(cardData);

            // data update to snappoint & move to empty slot
            _snapPoints[i].Set_CurrentCard(card);

            // controller data update
            _drawnCards.Add(card);

            // remove card on top of the deck
            _deckCards.RemoveAt(_deckCards.Count - 1);

            DeckCount_TextUpdate();

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
