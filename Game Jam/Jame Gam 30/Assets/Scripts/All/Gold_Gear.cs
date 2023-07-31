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
    public List<Tile> setTiles { get => _setTiles; set => _setTiles = value; }

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
        for (int i = 0; i < _setTiles.Count; i++)
        {
            if (_setTiles[i].currentGear == null) return;
            if (!_setTiles[i].currentGear.spinInActive) return;
        }

        _basicGear.currentTile.levelController.timeController.End_Game();
        DarkGear_Update();
        _basicGear.spinningRight = !_basicGear.spinningRight;

        for (int i = 0; i < _setTiles.Count; i++)
        {
            _setTiles[i].currentGear.Spin_Activation_Check(true);
        }
    }
}