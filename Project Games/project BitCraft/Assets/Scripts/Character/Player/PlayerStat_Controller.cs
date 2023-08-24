using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerStat_Controller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Components")]
    [SerializeField] private Game_Controller _gameController;
    [SerializeField] private GameObject _mainPanel;
    private Stat_Controller _playerStatController;

    [Header("LeanTween Value")]
    [SerializeField] private LeanTweenType _tweenType;
    [SerializeField] private float _panelSpeed;
    [SerializeField] private int _statCount;
    private bool _panelOnHold;

    [Header("Hold Icon")]
    [SerializeField] private Image _holdIcon;
    [SerializeField] private Sprite _holdSprite;
    [SerializeField] private Sprite _UnHoldSprite;

    [Header("InGame Value")]
    [SerializeField] private List<UI_Bar> _fatigueBars = new List<UI_Bar>();

    //
    public void OnPointerEnter(PointerEventData eventData)
    {
        Open_Panel();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Close_Panel();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Hold_Panel();
    }

    // Component Link
    public void Link_Player_StatController(Stat_Controller playerStatController)
    {
        _playerStatController = playerStatController;
    }

    // Panel Control
    private void Open_Panel()
    {
        Update_All_UIBar();

        LeanTween.cancel(_mainPanel);
        LeanTween.moveLocalY(_mainPanel, 75f * (_statCount - 1), _panelSpeed).setEase(_tweenType);
    }
    private void Close_Panel()
    {
        if (_panelOnHold) return;

        LeanTween.cancel(_mainPanel);
        LeanTween.moveLocalY(_mainPanel, 0f, _panelSpeed).setEase(_tweenType);
    }
    private void Hold_Panel()
    {
        if (!_panelOnHold)
        {
            _panelOnHold = true;
            _holdIcon.sprite = _holdSprite;
        }
        else
        {
            _panelOnHold = false;
            _holdIcon.sprite = _UnHoldSprite;
        }
    }

    // Stat Control
    public void Update_All_UIBar()
    {
        Update_Fatigue_UIBar();
    }

    public void Update_Fatigue_UIBar()
    {
        Stat_Controller psc = _playerStatController;

        int playerFatigue = psc.currentFatigue;
        int barCount = playerFatigue / _fatigueBars.Count;

        for (int i = 0; i < _fatigueBars.Count; i++)
        {
            if (barCount > 0)
            {
                _fatigueBars[i].Fill();
                barCount--;
                continue;
            }

            _fatigueBars[i].Empty();
        }
    }
}
