using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _rectTransform;
    public RectTransform rectTransform => _rectTransform;

    private MainController _main;
    public MainController main => _main;

    [Header("")]
    private Image _cardImage;
    [SerializeField] private Image _iconImage;

    private CardData _currentData;
    public CardData currentData => _currentData;


    private int _tiltTweenID;


    // UnityEngine
    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();

        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
        _cardImage = gameObject.GetComponent<Image>();
    }


    // UnityEngine.EventSystems
    public void OnPointerClick(PointerEventData eventData)
    {
        if (MainController.actionAvailable == false) return;

        Drag();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (MainController.actionAvailable == false) return;

        Tilt_QuarterRotate(0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }


    // Set Functions
    public void Set_CurrentData(CardData setData)
    {
        // current data set
        _currentData = setData;

        // card visual update
        _cardImage.sprite = setData.scrObj.cardSprite;
        _cardImage.color = Color.white;

        if (setData.scrObj.iconSprite != null)
        {
            _iconImage.sprite = setData.scrObj.iconSprite;
            _iconImage.color = Color.white;
        }
        else _iconImage.color = Color.clear;
    }

    public void Drag()
    {
        // if same card
        if (_main.cursor.dragCard == this)
        {
            Return();
            return;
        }

        // if another card dragging
        if (_main.cursor.isDragging)
        {
            _main.cursor.dragCard.Return();
        }

        // get this current card's prefab
        _main.cursor.Drag_Card(this);

        // animation
        LeanTween.moveLocal(gameObject, new Vector2(transform.localPosition.x, transform.localPosition.y + 25f), 0.1f).setEase(_main.cards.moveType);
        LeanTween.alpha(_rectTransform, 0.1f, 0.1f);
        _iconImage.color = Color.clear;

        Tilt_QuarterRotate(0.1f);
    }

    public void Return()
    {
        _main.cursor.Clear_Card();

        // animation
        LeanTween.moveLocal(gameObject, new Vector2(transform.localPosition.x, transform.localPosition.y - 25f), 0.1f).setEase(_main.cards.moveType);
        Set_CurrentData(_currentData);

        Tilt_QuarterRotate(0.1f);
    }

    public void Use()
    {
        _main.cards.Remove_DrawnCard(this);
    }


    // Animation
    public void Moveto_Destination(Transform destination)
    {
        LeanTween.move(gameObject, destination.position, _main.cards.movementTime).setEase(_main.cards.moveType);
    }
    public void Moveto_Destination(Vector2 destination)
    {
        LeanTween.move(gameObject, destination, _main.cards.movementTime).setEase(_main.cards.moveType);
    }

    /// <summary>
    /// Tilted to 0
    /// </summary>
    public void Tilt_QuarterRotate(float delayTime)
    {
        // reset leantween if currenlty playing
        if (_tiltTweenID != -1)
        {
            LeanTween.cancel(gameObject, _tiltTweenID);
            _tiltTweenID = -1;
        }

        // set starting rotation
        transform.Rotate(0f, 0f, 10f);

        // leantween rotate to 0
        _tiltTweenID = LeanTween.rotateZ(gameObject, 0f, 0.75f).setEase(_main.cards.tiltType).setDelay(delayTime).id;
    }

    /// <summary>
    /// 0 to Tilted
    /// </summary>
    public void TiltOpposite_QuarterRotate()
    {
        LeanTween.rotateZ(gameObject, -10f, 0.75f).setEase(_main.cards.tiltType);
    }
}
