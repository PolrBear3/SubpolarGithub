using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Bar : MonoBehaviour
{
    private Image _image;

    private bool _filled;
    public bool filled { get => _filled; set => _filled = value; }

    [SerializeField] private Sprite _emptyBar;
    [SerializeField] private Sprite _filledBar;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Image image)) _image = image;
    }

    //
    public void Fill()
    {
        _filled = true;
        _image.sprite = _filledBar;
    }
    public void Empty()
    {
        _filled = false;
        _image.sprite = _emptyBar;
    }
}
