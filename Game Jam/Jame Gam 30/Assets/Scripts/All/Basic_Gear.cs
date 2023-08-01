using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_Gear : MonoBehaviour
{
    private Tile _currentTile;
    public Tile currentTile { get => _currentTile; set => _currentTile = value; }

    [SerializeField] private float _spinSpeed;

    [SerializeField] private bool _unHoldable;
    public bool unHoldable { get => _unHoldable; set => _unHoldable = value; }

    [SerializeField] private bool _spinningRight;
    public bool spinningRight { get => _spinningRight; set => _spinningRight = value; }

    private bool _spinInActive;
    public bool spinInActive { get => _spinInActive; set => _spinInActive = value; }

    private Object_Gear _objectGear;
    public Object_Gear objectGear { get => _objectGear; set => _objectGear = value; }

    private Gold_Gear _goldGear;
    public Gold_Gear goldGear { get => _goldGear; set => _goldGear = value; }

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
        _currentTile.indicator.Deactivate_Animation();

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
    public void Spin_Activation_Check(bool refresh)
    {
        if (_goldGear != null || _objectGear != null) return;

        List<Tile> surroundingTiles = _currentTile.levelController.Surrounding_Tiles(_currentTile);

        if (refresh)
        {
            for (int i = 0; i < surroundingTiles.Count; i++)
            {
                if (surroundingTiles[i].currentGear == null) continue;
                if (surroundingTiles[i].currentGear.goldGear != null) continue;
                if (surroundingTiles[i].currentGear.objectGear != null) continue;
                if (_spinningRight != surroundingTiles[i].currentGear.spinningRight) continue;

                _spinInActive = true;
                _currentTile.indicator.Spin_InActive_Animation(_spinningRight);
                return;
            }
        }
        else
        {
            for (int i = 0; i < surroundingTiles.Count; i++)
            {
                if (surroundingTiles[i].currentGear == null) continue;
                if (surroundingTiles[i].currentGear.goldGear != null) continue;
                if (surroundingTiles[i].currentGear.objectGear != null) continue;
                if (!surroundingTiles[i].currentGear.spinInActive && _spinningRight != surroundingTiles[i].currentGear.spinningRight) continue;

                _spinInActive = true;
                _currentTile.indicator.Spin_InActive_Animation(_spinningRight);
                return;
            }
        }

        _spinInActive = false;
        _currentTile.indicator.Deactivate_Animation();
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
