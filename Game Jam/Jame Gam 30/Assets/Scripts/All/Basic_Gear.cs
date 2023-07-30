using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_Gear : MonoBehaviour
{
    private Tile _currentTile;
    public Tile currentTile { get => _currentTile; set => _currentTile = value; }

    [SerializeField] private float _spinSpeed;

    [SerializeField] private bool _spinningRight;
    public bool spinningRight { get => _spinningRight; set => _spinningRight = value; }

    private bool _spinInActive;
    public bool spinInActive { get => _spinInActive; set => _spinInActive = value; }

    //
    private void Awake()
    {
        Set_CurrentTile();
    }
    private void FixedUpdate()
    {
        Spin();
    }

    // Set
    private void Set_CurrentTile()
    {
        if (!transform.parent.TryGetComponent(out Tile tile)) return;

        _currentTile = tile;
        _currentTile.currentGear = this;
    }

    // Action
    public void Move_toPlayer()
    {
        _spinInActive = false;

        Player_Movement player = _currentTile.levelController.gameController.currentPlayer;

        _currentTile.currentGear = null;
        _currentTile = null;

        transform.parent = player.holdPoint;
        LeanTween.moveLocal(gameObject, Vector2.zero, 0.5f).setEase(LeanTweenType.easeInOutQuint);
    }
    public void Move_toTile(Tile targetTile)
    {
        _currentTile = targetTile;
        _currentTile.currentGear = this;

        transform.parent = _currentTile.transform;
        LeanTween.moveLocal(gameObject, Vector2.zero, 0.5f).setEase(LeanTweenType.easeInOutQuint);
    }

    // Spin Update
    public void Spin_Activation_Check()
    {
        List<Tile> surroundingTiles = _currentTile.levelController.Surrounding_Tiles(_currentTile);

        bool inActive = false;

        for (int i = 0; i < surroundingTiles.Count; i++)
        {
            if (surroundingTiles[i].currentGear == null) continue;
            if (!surroundingTiles[i].currentGear.spinInActive && _spinningRight != surroundingTiles[i].currentGear.spinningRight) continue;
            else inActive = true;
        }

        _spinInActive = inActive;
    }
    private void Spin()
    {
        if (_spinInActive) return;

        if (_spinningRight)
        {
            transform.Rotate(0, 0, -_spinSpeed);
        }
        else
        {
            transform.Rotate(0, 0, _spinSpeed);
        }
    }
}
