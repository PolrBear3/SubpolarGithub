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

    private Card _interactedCard;
    public Card interactedCard => _interactedCard;

    private Coroutine _fillBarCoroutine;


    public Action<Card> OnInteract;

    public Action OnFillbarComplete;


    // MonoBehaviour
    private void Start()
    {
        _cardPointer.SetActive(false);

        _card.data.SetMax_FillBarValue(20);
        _card.data.SetCurrent_FillBarValue(20);

        Update_FillBar();
        Update_FillBar(0);

        // subscriptions
    }


    // IInteractCondition (use on custom card components)
    public bool Interactable()
    {
        return false;
    }


    // From Other Card Pointer
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
    
    
    // Current Card Pointer
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


    // Indicators
    public void Update_FillBar()
    {
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
        Update_FillBar();

        OnFillbarComplete?.Invoke();

        _fillBarCoroutine = null;
        yield break;
    }
}