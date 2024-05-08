using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardsController : MonoBehaviour
{
    [Header("")]
    [SerializeField] private MainController _main;
    public MainController main => _main;

    [Header("")]
    [SerializeField] private RectTransform _addToDeckSpawnPoint;
    [SerializeField] private RectTransform _spawnPoint;

    [SerializeField] private Card_SnapPoint[] _snapPoints;
    public Card_SnapPoint[] snapPoints => _snapPoints;

    private List<CardScrObj> _deckCards = new();
    private List<Card> _drawnCards = new();

    [Header("")]
    [SerializeField] private TextMeshProUGUI _deckCountText;

    [Header("")]
    [SerializeField] private Image _deckSpriteRenderer;
    [SerializeField] private Sprite[] _deckSprites;


    [Header("")]
    [SerializeField] [Range(0, 10)] private int _maxDrawCardAmount;
    public int maxDrawCardAmount => _maxDrawCardAmount;

    [Header("")]
    [SerializeField] private LeanTweenType _tiltType;
    public LeanTweenType tiltType => _tiltType;

    [SerializeField] private LeanTweenType _moveType;
    public LeanTweenType moveType => _moveType;

    [SerializeField] private float _movementTime;
    public float movementTime => _movementTime;

    [SerializeField] private float _reorderTime;

    [Header("")]
    [SerializeField] [Range(0, 10)] private int _randCardPrice;
    [SerializeField] [Range(0, 10)] private int _landCardPrice;
    [SerializeField] [Range(0, 10)] private int _buffCardPrice;

    [Header("")]
    [SerializeField] [Range(0, 10)] private int _buyCardAmount;


    // UnityEngine
    private void Start()
    {
        DeckCount_TextUpdate();
        Deck_SpriteUpdate();
    }


    // Deck Control
    public void AddCard_toDeck(CardScrObj addCard)
    {
        _deckCards.Add(addCard);

        DeckCount_TextUpdate();
        Deck_SpriteUpdate();
    }
    public void AddCards_toDeck(List<CardScrObj> addCards)
    {
        for (int i = 0; i < addCards.Count; i++)
        {
            AddCard_toDeck(addCards[i]);
        }
    }

    private void DeckCount_TextUpdate()
    {
        _deckCountText.text = _deckCards.Count.ToString();

        // calculate sprite number
        int spriteNum = _deckCards.Count / _maxDrawCardAmount;
        spriteNum = Mathf.Clamp(spriteNum, 0, _deckSprites.Length - 1);

        float startingY = -25f;
        float setY = startingY + (6 * spriteNum);

        // update position
        _deckCountText.rectTransform.LeanSetLocalPosY(setY);
    }

    private void Deck_SpriteUpdate()
    {
        // calculate sprite number
        int spriteNum = _deckCards.Count / _maxDrawCardAmount;
        spriteNum = Mathf.Clamp(spriteNum, 0, _deckSprites.Length - 1);

        // update sprite
        _deckSpriteRenderer.sprite = _deckSprites[spriteNum];
    }


    // Shop
    public void Buy_RandomCard()
    {
        if (MainController.actionAvailable == false) return;
        if (_main.gameData.overallPopulation < _randCardPrice) return;

        BuyCard(_main.data.AllCard_ScrObjs(_buyCardAmount));

        _main.Update_OverallPopulation(-_randCardPrice);
    }

    public void Buy_LandCard()
    {
        if (MainController.actionAvailable == false) return;
        if (_main.gameData.overallPopulation < _landCardPrice) return;

        BuyCard(_main.data.AllLandCard_ScrObjs(_buyCardAmount));

        _main.Update_OverallPopulation(-_landCardPrice);
    }

    public void Buy_BuffCard()
    {
        if (MainController.actionAvailable == false) return;
        if (_main.gameData.overallPopulation < _buffCardPrice) return;

        BuyCard(_main.data.AllBuffCard_ScrObjs(_buyCardAmount));

        _main.Update_OverallPopulation(-_buffCardPrice);
    }


    private void BuyCard(List<CardScrObj> boughtCards)
    {
        StartCoroutine(BuyCard_Coroutine(boughtCards));
    }
    private IEnumerator BuyCard_Coroutine(List<CardScrObj> boughtCards)
    {
        MainController.actionAvailable = false;

        for (int i = 0; i < boughtCards.Count; i++)
        {
            GameObject spawnCard = Instantiate(_main.data.cardPrefab, _addToDeckSpawnPoint);

            Card card = spawnCard.GetComponent<Card>();
            CardData cardData = new(boughtCards[i]);

            card.Set_CurrentData(cardData);
            AddCard_toDeck(boughtCards[i]);

            // Animation
            Vector2 addToDeckPoint = new(_addToDeckSpawnPoint.transform.position.x, _deckSpriteRenderer.transform.position.y - 0.45f);

            // card.Tilt_QuarterRotate(_movementTime); //
            card.Moveto_Destination(addToDeckPoint);
            yield return new WaitForSeconds(_movementTime * 2);

            Destroy(spawnCard);
        }

        MainController.actionAvailable = true;
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

            // animation
            card.Tilt_QuarterRotate(_movementTime);

            //
            DeckCount_TextUpdate();
            Deck_SpriteUpdate();

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
        StartCoroutine(ReturnDrawnCards_toDeck_Coroutine());
    }
    private IEnumerator ReturnDrawnCards_toDeck_Coroutine()
    {
        for (int i = _drawnCards.Count - 1; i >= 0; i--)
        {
            AddCard_toDeck(_drawnCards[i].currentData.scrObj);

            // move to deck animation
            _drawnCards[i].TiltOpposite_QuarterRotate();
            _drawnCards[i].Moveto_Destination(_spawnPoint);

            yield return new WaitForSeconds(_movementTime);

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

            // skip if nothing changed
            if (_snapPoints[i].currentCard == _drawnCards[i]) continue;

            _snapPoints[i].Set_CurrentCard(_drawnCards[i]);

            // card reorder animation
            _drawnCards[i].Tilt_QuarterRotate(_reorderTime);
        }
    }
}
