using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold_Gear : MonoBehaviour
{
    private Basic_Gear _basicGear;
    public Basic_Gear basicGear { get => _basicGear; set => _basicGear = value; }

    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _darkGearSprite;

    [SerializeField] private List<Tile> _setTiles = new List<Tile>();

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) _spriteRenderer = sr;
        Set_CurrentTile();
        SetTiles_Indication();
    }

    // Set
    private void Set_CurrentTile()
    {
        if (gameObject.TryGetComponent(out Basic_Gear gear)) _basicGear = gear;
        _basicGear.goldGear = this;

        if (_basicGear.currentTile == null) return;
        _basicGear.currentTile.levelController.goldGear = this;
    }
    private void SetTiles_Indication()
    {
        for (int i = 0; i < _setTiles.Count; i++)
        {
            _setTiles[i].indicator.GoldGear_Indication();
        }
    }

    // Sprite
    public void DarkGear_Update()
    {
        _spriteRenderer.sprite = _darkGearSprite;
    }

    // Update
    public void SpinReverse_Check()
    {
        List<Basic_Gear> surroundingGears = new List<Basic_Gear>();

        for (int i = 0; i < _setTiles.Count; i++)
        {
            if (_setTiles[i].currentGear == null) return;
            surroundingGears.Add(_setTiles[i].currentGear);

            if (_setTiles[i].currentGear.spinInActive) return;
            if (_basicGear.spinningRight != _setTiles[i].currentGear.spinningRight) return;
        }

        _basicGear.currentTile.levelController.timeController.End_Game();
        _basicGear.spinningRight = !_basicGear.spinningRight;
        DarkGear_Update();

        for (int i = 0; i < surroundingGears.Count; i++)
        {
            surroundingGears[i].Spin_Activation_Check(true);
        }
    }
}