using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractCondition
{
    public bool Interactable(Card interactCheckCard);
}

public class Card_Interaction : MonoBehaviour, IInteractCondition
{
    [Space(20)]
    [SerializeField] private Card _card;

    [SerializeField] private GameObject _cardPointer;
    public GameObject cardPointer => _cardPointer;

    [Space(20)]
    [SerializeField] private GameObject _iconBox;
    [SerializeField] private SpriteRenderer _iconSR;

    [Space(20)]
    [SerializeField] private GameObject _barBox;
    [SerializeField] private SpriteRenderer _fillBarSR;

    [Space(20)]
    [SerializeField][Range(0, 10)] private float _barMinWidth;
    [SerializeField][Range(0, 10)] private float _barMaxWidth;

    [Space(20)]
    [SerializeField][Range(0, 10)] private float _fillBarSpeed;


    private Card _pointingCard;
    public Card pointingCard => _pointingCard;

    private Card _interactedCardFlag;
    public Card interactedCardFlag => _interactedCardFlag;

    public Action<Card> OnInteract;

    private Coroutine _fillBarCoroutine;
    public Action OnFillbarComplete;


    // MonoBehaviour
    private void Start()
    {
        _cardPointer.SetActive(false);

        Update_Icon(null);
        Update_FillBar(false);

        // subscriptions (Interaction Examples)
        OnInteract += Launch_AdditionalCards;
    }


    // IInteractCondition (use on custom card components)
    public bool Interactable(Card droppedCard)
    {
        return InteractCard_Match(droppedCard);
    }


    // Pointed From Other Card
    public void Reset_InteractData()
    {
        if (_card.movement.dragging) return;

        _pointingCard = null;
        _interactedCardFlag = null;
    }
    public void Interact_PointedCard()
    {
        if (_card.movement.dragging) return;

        if (_pointingCard == null) return;
        if (Card_Interactable(_pointingCard) == false) return;

        _interactedCardFlag = _pointingCard;

        if (Interactable(_interactedCardFlag) == false) return;
        OnInteract?.Invoke(_interactedCardFlag);
    }
    
    
    // Current Card Pointer
    private bool Card_Interactable(Card card)
    {
        if (!card.gameObject.TryGetComponent(out IInteractCondition interactCondition)) return false;
        return interactCondition.Interactable(_card);
    }

    public void Point_ClosestCard()
    {
        _pointingCard = null;

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


    // Indicators
    public void Update_Icon(Sprite iconSprite)
    {
        bool toggle = iconSprite != null;
        _iconBox.SetActive(toggle);

        if (toggle == false) return;
        _iconSR.sprite = iconSprite;
    }


    public void Update_FillBar(bool toggle)
    {
        _barBox.SetActive(toggle);

        if (toggle == false) return;

        Card_Data data = _card.data;

        float widthStep = _barMaxWidth / data.maxFillBarValue;
        float targetWidth = widthStep * data.currentFillBarValue;

        float restrictedSize = targetWidth >= _barMinWidth || targetWidth <= 0f ? targetWidth : _barMinWidth;
        Vector2 barSize = new(restrictedSize, _fillBarSR.size.y);

        _fillBarSR.size = barSize;
    }

    public void Update_FillBar(int targetValue)
    {
        if (_fillBarCoroutine != null)
        {
            StopCoroutine(_fillBarCoroutine);
            _fillBarCoroutine = null;
        }

        Card_Data data = _card.data;
        if (targetValue == data.currentFillBarValue) return;

        _fillBarCoroutine = StartCoroutine(FillBar_UpdateCoroutine(targetValue));
    }
    private IEnumerator FillBar_UpdateCoroutine(int targetValue)
    {
        Card_Data data = _card.data;

        float widthStep = _barMaxWidth / data.maxFillBarValue;
        float targetWidth = widthStep * targetValue;

        while (Mathf.Abs(_fillBarSR.size.x - targetWidth) > 0.001f && _fillBarSR.size.x >= _barMinWidth)
        {
            float moveStep = Time.deltaTime * _fillBarSpeed * 0.1f;
            float updateWidth = Mathf.MoveTowards(_fillBarSR.size.x, targetWidth, moveStep);

            _fillBarSR.size = new Vector2(updateWidth, _fillBarSR.size.y);
            yield return null;
        }

        data.SetCurrent_FillBarValue(targetValue);
        Update_FillBar(true);

        OnFillbarComplete?.Invoke();

        _fillBarCoroutine = null;
        yield break;
    }


    // Interaction Examples
    private bool InteractCard_Match(Card compareCard)
    {
        if (compareCard == null) return false;
        Card_ScrObj interactCard = compareCard.data.cardScrObj;

        return _card.data.cardScrObj == interactCard;
    }

    public void Launch_AdditionalCards(Card interactedCard)
    {
        Game_Controller controller = Game_Controller.instance;
        TableTop tableTop = controller.tableTop;

        Vector2 interactCardPos = interactedCard.transform.position;

        List<Vector2> spawnPositions = tableTop.Surrounding_CardSnapPoints(interactCardPos);
        List<Vector2> launchPositions = tableTop.SurroundingSeperated_CardSnapPoints(interactCardPos);

        int launchCardCountExample = 3;

        for (int i = 0; i < launchCardCountExample; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, spawnPositions.Count);
            Vector2 spawnPos = spawnPositions[randIndex];

            Vector2 launchPos = Utility.ClosestConverted_Positions(spawnPos, launchPositions)[0];
            launchPos = tableTop.Is_OuterGrid(launchPos) ? tableTop.CardSnapPoints(launchPos)[0] : launchPos;

            Card spawnedCard = _card.cardLauncher.Launch_Card(spawnPos, launchPos);
            spawnPositions.RemoveAt(randIndex);

            spawnedCard.movement.Push_OverlappedCards();

            // set data
            tableTop.Track_CurrentCard(spawnedCard);
            spawnedCard.transform.SetParent(tableTop.allCards);

            Card_ScrObj[] startingCards = tableTop.startingCards;
            int cardIndex = UnityEngine.Random.Range(0, startingCards.Length);

            spawnedCard.Set_Data(new(startingCards[cardIndex]));

            spawnedCard.Update_Visuals();
            spawnedCard.movement.Update_Shadows();
        }
    }
}