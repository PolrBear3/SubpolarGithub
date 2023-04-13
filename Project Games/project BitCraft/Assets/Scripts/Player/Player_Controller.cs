using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private TileMap_Controller _mapController;
    public TileMap_Controller mapController { get => _mapController; set => _mapController = value; }
    
    private int _currentRowNum;
    public int currentRowNum { get => _currentRowNum; set => _currentRowNum = value; }

    private int _currentColumnNum;
    public int currentColumnNum { get => _currentColumnNum; set => _currentColumnNum = value; }

    private bool _moveReady = false;
    public bool moveReady { get => _moveReady; set => _moveReady = value; }

    public void Set_Data(TileMap_Controller mapController)
    {
        this.mapController = mapController;
    }
    public void Update_Position(int row, int column)
    {
        currentRowNum = row;
        currentColumnNum = column;
    }

    public void Click()
    {
        if (!moveReady) 
        {
            moveReady = true;
            mapController.actionSystem.Highlight_Player_Moveable_Tiles();
        }
        else
        {
            moveReady = false;
            mapController.actionSystem.UnHighlight_All_tiles();
        }
    }

    public void Move()
    {
        // lean tween to vector zero
        transform.localPosition = Vector2.zero;
    }
}
