using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Object_Scanner : MonoBehaviour
{
    private Drag_Slot _dragSlot;

    [Header("Main")]
    [SerializeField] private float _hoverTime;
    public float hoverTime { get => _hoverTime; set => _hoverTime = value; }

    [Header ("Amount")]
    [SerializeField] private Image _droppedIcon; 
    [SerializeField] private Text _droppedText;

    [Header("Health")]
    [SerializeField] private Image _lifeIcon;
    [SerializeField] private Text _lifeText;

    private Prefab_Controller _scannedPrefab;
    public Prefab_Controller scannedPrefab { get => _scannedPrefab; set => _scannedPrefab = value; }

    private bool _objectDetected;
    public bool objectDetected { get => _objectDetected; set => _objectDetected = value; }

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Drag_Slot dragSlot)) { _dragSlot = dragSlot; }
    }


    //
    public void Hide_All()
    {
        Hide_Amount();
        Hide_Life();
    }

    public void Show_Amount(Prefab_Controller scannedPrefab)
    {
        if (_dragSlot.itemDragging) return;
        if (scannedPrefab == null) return;

        _scannedPrefab = scannedPrefab;
        _objectDetected = true;

        // icon
        _droppedIcon.color = Color.white;

        // text
        Color textColor = _droppedText.color;
        _droppedText.text = scannedPrefab.currentAmount.ToString();
        textColor.a = 1f;
        _droppedText.color = textColor;
    }
    public void Update_Amount()
    {

    }
    public void Hide_Amount()
    {
        _scannedPrefab = null;
        _objectDetected = false;

        // icon
        _droppedIcon.color = Color.clear;

        // text
        Color textColor = _droppedText.color;
        textColor.a = 0f;
        _droppedText.color = textColor;
    }

    public void Show_Life(Prefab_Controller scannedPrefab)
    {
        if (_dragSlot.itemDragging) return;
        if (scannedPrefab == null) return;

        _scannedPrefab = scannedPrefab;
        _objectDetected = true;

        // icon
        _lifeIcon.color = Color.white;

        // text
        Color textColor = _lifeText.color;
        _lifeText.text = scannedPrefab.statController.currentLifeCount.ToString();
        textColor.a = 1f;
        _lifeText.color = textColor;
    }
    public void Update_Life()
    {
        Show_Life(_scannedPrefab);
    }
    public void Hide_Life()
    {
        _scannedPrefab = null;
        _objectDetected = false;

        // icon
        _lifeIcon.color = Color.clear;

        // text
        Color textColor = _lifeText.color;
        textColor.a = 0f;
        _lifeText.color = textColor;
    }
}