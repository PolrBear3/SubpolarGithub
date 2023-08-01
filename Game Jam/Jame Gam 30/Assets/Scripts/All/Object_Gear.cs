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

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Basic_Gear basicGear)) _basicGear = basicGear;
        _basicGear.objectGear = this;
    }

    //
    private bool Activate_Available()
    {
        List<Tile> surroundingTiles = _basicGear.currentTile.levelController.Surrounding_Tiles(_basicGear.currentTile);

        for (int i = 0; i < surroundingTiles.Count; i++)
        {
            if (surroundingTiles[i].currentGear == null) continue;
            if (surroundingTiles[i].currentGear.spinInActive) continue;
            if (_basicGear.spinningRight != surroundingTiles[i].currentGear.spinningRight) continue;
            return true;
        }
        return false;
    }

    public void Activate_Object()
    {
        if (!Activate_Available()) return;
        if (!_object.TryGetComponent(out IActivate activate)) return;

        activate.Activate();
        _basicGear.spinningRight = !_basicGear.spinningRight;
    }
}