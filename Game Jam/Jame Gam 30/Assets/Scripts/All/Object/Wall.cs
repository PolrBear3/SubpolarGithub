using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IActivate
{
    private Tile _currentTile;
    public Tile currentTile { get => _currentTile; set => _currentTile = value; }

    private bool _opened;

    [SerializeField] private bool _openLeft;
    [SerializeField] private int _openRange;
    [SerializeField] private float _openSpeed;

    //
    private void Awake()
    {
        if (transform.parent.TryGetComponent(out Tile currentTile)) _currentTile = currentTile;
    }
    public void Activate()
    {
        if (!_opened) Open();
        else Close();
    }

    //
    private void Open()
    {
        _opened = true;

        int openNum;
        if (_openLeft) openNum = -_openRange;
        else openNum = _openRange;

        Tile moveTile = _currentTile.levelController.Get_Tile(_currentTile.xPosition + openNum, _currentTile.yPosition);
        _currentTile = moveTile;

        transform.parent = moveTile.transform;
        LeanTween.moveLocalX(gameObject, 0f, _openSpeed);
    }
    private void Close()
    {
        _opened = false;

        int openNum;
        if (_openLeft) openNum = _openRange;
        else openNum = -_openRange;

        Tile moveTile = _currentTile.levelController.Get_Tile(_currentTile.xPosition + openNum, _currentTile.yPosition);
        _currentTile = moveTile;

        transform.parent = moveTile.transform;
        LeanTween.moveLocalX(gameObject, 0f, _openSpeed);
    }
}
