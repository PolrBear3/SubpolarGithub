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

    [Header("")]
    [SerializeField] [Range(0, 10)] private int _randCardPrice;
    [SerializeField] [Range(0, 10)] private int _landCardPrice;
    [SerializeField] [Range(0, 10)] private int _buffCardPrice;

    [Header("")]
    [SerializeField] [Range(0, 10)] private int _cardBundleAmount;


    // UnityEngine
    private void Start()
    {
        DeckCount_TextUpdate();
    }


    // Deck Control
    public void AddCard_toDeck(CardScrObj addCard)
    {
        _deckCards.Add(addCard);
        DeckCount_TextUpdate();
    }
    public void AddCard_toDeck(List<CardScrObj> addCards)
    {
        for (int i = 0; i < addCards.Count; i++)
        {
            AddCard_toDeck(addCards[i]);
        }
    }

    private void DeckCount_TextUpdate()
    {
        _deckCountText.text = _deckCards.Count.ToString();
    }


    // Shop
    public void Buy_RandomCard()
    {
        if (_main.gameData.overallPopulation < _randCardPrice) return;

        AddCard_toDeck(_main.data.AllCard_ScrObjs(_cardBundleAmount));

        _main.Update_OverallPopulation(-_randCardPrice);
    }

    public void Buy_LandCard()
    {
        if (_main.gameData.overallPopulation < _landCardPrice) return;

        AddCard_toDeck(_main.data.AllLandCard_ScrObjs(_cardBundleAmount));

        _main.Update_OverallPopulation(-_landCardPrice);
    }

    public void Buy_BuffCard()
    {
        if (_main.gameData.overallPopulation < _buffCardPrice) return;

        AddCard_toDeck(_main.data.AllBuffCard_ScrObjs(_cardBundleAmount));

        _main.Update_OverallPopulation(-_buffCardPrice);
    }


    // Draw Card Control
    public void DrawCard_fromDeck()
    {
        if (_deckCards.Count <= 0) return;

        for (int i = 0; i < _snapPoints.Length; i++)
        {
            if (_snapPoints[i].hasCard) continue;

            GameObject spawnCard = Instantiate(_main.data.cardPrefab, _spawnPoint);

            int randCardArrayNum = Random.Range(0, _deckCards.Count - 1);

            Card card = spawnCard.GetComponent<Card>();
            CardData cardData = new(_deckCards[randCardArrayNum]);

            // new data set to drawn card
            card.Set_CurrentData(cardData);

            // data update to snappoint & move to empty slot
            _snapPoints[i].Set_CurrentCard(card);

            // controller data update
            _drawnCards.Add(card);

            // remove card on top of the deck
            _deckCards.RemoveAt(randCardArrayNum);

            DeckCount_TextUpdate();

            return;
        }
    }
    public void DrawCard_fromDeck(int drawAmount)
    {
        for (int i = 0; i < drawAmount; i++)
        {
            DrawCard_fromDeck();
        }
    }

    public void Remove_DrawnCard(Card removeCard)
    {
        _drawnCards.Remove(removeCard);
        Destroy(removeCard.gameObject);

        Reorder_DrawnCards();
    }


    public void ReturnDrawnCards_toDeck()
    {
        for (int i = 0; i < _drawnCards.Count; i++)
        {
            AddCard_toDeck(_drawnCards[i].currentData.scrObj);
            Remove_DrawnCard(_drawnCards[i]);
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
}
