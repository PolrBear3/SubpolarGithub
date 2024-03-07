using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Level_Controller _levelController;
    public Level_Controller levelController { get => _levelController; set => _levelController = value; }

    [SerializeField] private Tile_Indicator _indicator;
    public Tile_Indicator indicator { get => _indicator; set => _indicator = value; }

    [SerializeField] private int _xPosition;
    public int xPosition { get => _xPosition; set => _xPosition = value; }

    [SerializeField] private int _yPosition;
    public int yPosition { get => _yPosition; set => _yPosition = value; }

    private bool _hasObject;
    public bool hasObject { get => _hasObject; set => _hasObject = value; }

    [SerializeField] private bool _isDefaultTile;
    public bool isDefaultTile { get => _isDefaultTile; set => _isDefaultTile = value; }

    private Basic_Gear _currentGear;
    public Basic_Gear currentGear { get => _currentGear; set => _currentGear = value; }

    // Set
    public void Set_Data(Level_Controller levelController, int x, int y)
    {
        _levelController = levelController;
        _xPosition = x;
        _yPosition = y;
    }
}
