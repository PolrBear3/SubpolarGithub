using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_Gear : MonoBehaviour
{
    [SerializeField] private Tile _currentTile;
    public Tile currentTile { get => _currentTile; set => _currentTile = value; }

    [SerializeField] private float _spinSpeed;

    [SerializeField] private bool _spinInActive;
    public bool spinInActive { get => _spinInActive; set => _spinInActive = value; }

    private bool _spinningRight;
    public bool spinningRight { get => _spinningRight; set => _spinningRight = value; }

    //
    private void Awake()
    {
        Set_CurrentTile();
        Spin_Activation_Check();
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
        Player_Movement player = _currentTile.levelController.gameController.currentPlayer;

        _spinInActive = false;

        Spin_Activation_Check();

        _currentTile.currentGear = null;
        _currentTile = null;
        player.currentGear = this;

        transform.parent = player.holdPoint;
        LeanTween.moveLocal(gameObject, Vector2.zero, 0.5f).setEase(LeanTweenType.easeInOutQuint);
    }

    public void Move_toTile(Tile targetTile)
    {
        if (targetTile.currentGear != null || targetTile.hasObject) return;

        _currentTile = targetTile;
        targetTile.currentGear = this;

        transform.parent = _currentTile.transform;
        LeanTween.moveLocal(gameObject, Vector2.zero, 0.5f).setEase(LeanTweenType.easeInOutQuint);

        Spin_Activation_Check();
    }
    public void Move_toTile(int toX, int toY)
    {
        Tile nextTile = _currentTile.levelController.Get_Tile(toX, toY);
        if (nextTile.currentGear != null || nextTile.hasObject) return;

        _currentTile = nextTile;
        nextTile.currentGear = this;

        transform.parent = _currentTile.transform;
        LeanTween.moveLocal(gameObject, Vector2.zero, 0.5f).setEase(LeanTweenType.easeInOutQuint);

        Spin_Activation_Check();
    }

    // Spin Update
    public void Spin_Activation_Check()
    {
        List<Tile> surroundingTiles = _currentTile.levelController.Surrounding_Tiles(_currentTile);

        for (int i = 0; i < surroundingTiles.Count; i++)
        {
            if (surroundingTiles[i].currentGear == null) continue;

            if (_spinningRight == surroundingTiles[i].currentGear.spinningRight)
            {
                _spinInActive = true;
                surroundingTiles[i].currentGear.spinInActive = true;
            }
            else
            {
                _spinInActive = false;
                surroundingTiles[i].currentGear.spinInActive = false;
            }
        }
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
