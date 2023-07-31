using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold_Gear : MonoBehaviour
{
    private Basic_Gear _basicGear;
    public Basic_Gear basicGear { get => _basicGear; set => _basicGear = value; }

    private SpriteRenderer _spriteRenderer;

    [SerializeField] private Sprite _darkGearSprite;
    [SerializeField] private int _spinPower;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) _spriteRenderer = sr;
        Set_CurrentTile();
    }

    // Set
    private void Set_CurrentTile()
    {
        if (gameObject.TryGetComponent(out Basic_Gear gear)) _basicGear = gear;
        _basicGear.goldGear = this;

        if (_basicGear.currentTile == null) return;
        _basicGear.currentTile.levelController.goldGear = this;
    }

    // Sprite
    public void DarkGear_Update()
    {
        _spriteRenderer.sprite = _darkGearSprite;
    }

    // Update
    public void SpinPower_Check()
    {
        Tile currentTile = _basicGear.currentTile;
        List<Tile> surroundingTiles = currentTile.levelController.Surrounding_Tiles(currentTile);
        List<Basic_Gear> checkGears = new List<Basic_Gear>();

        int powerCount = 0;
        for (int i = 0; i < surroundingTiles.Count; i++)
        {
            if (surroundingTiles[i].currentGear == null) continue;
            checkGears.Add(surroundingTiles[i].currentGear);

            if (!surroundingTiles[i].currentGear.spinInActive) continue;
            powerCount++;
        }

        if (powerCount < _spinPower) return;

        _basicGear.currentTile.levelController.timeController.End_Game();
        DarkGear_Update();
        _basicGear.spinningRight = !_basicGear.spinningRight;

        for (int i = 0; i < checkGears.Count; i++)
        {
            checkGears[i].Spin_Activation_Check(true);
        }
    }
}