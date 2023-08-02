using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivate
{
    void Activate();
}

public class Object_Gear : MonoBehaviour
{
    private Basic_Gear _basicGear;
    public Basic_Gear basicGear { get => _basicGear; set => _basicGear = value; }

    [SerializeField] private GameObject _object;

    [SerializeField] private List<Tile> _setTiles = new List<Tile>();

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Basic_Gear basicGear)) _basicGear = basicGear;
        _basicGear.objectGear = this;

        SetTiles_Indication();
    }

    //
    private void SetTiles_Indication()
    {
        for (int i = 0; i < _setTiles.Count; i++)
        {
            _setTiles[i].indicator.ObjectGear_Indication();
        }
    }

    public void Activate_Object()
    {
        for (int i = 0; i < _setTiles.Count; i++)
        {
            if (_setTiles[i].currentGear == null) return;
            if (_setTiles[i].currentGear.spinInActive) return;
            if (_basicGear.spinningRight != _setTiles[i].currentGear.spinningRight) return;
        }

        if (!_object.TryGetComponent(out IActivate activate)) return;

        activate.Activate();
        _basicGear.spinningRight = !_basicGear.spinningRight;

        _basicGear.currentTile.levelController.gameController.soundController.Play_Sound(2);
    }
}