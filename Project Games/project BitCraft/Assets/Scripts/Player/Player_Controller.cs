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

    private bool _interactReady = false;
    public bool interactReady { get => _interactReady; set => _interactReady = value; }

    [SerializeField] private float _moveSpeed;
    public float moveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

    // checks
    public bool Position_at_Crust()
    {
        List<Tile_Controller> crustTiles = mapController.combinationSystem.Map_Crust();

        for (int i = 0; i < crustTiles.Count; i++)
        {
            if (!crustTiles[i].Has_Prefab_ID(Prefab_Type.character, 0)) continue;
            return true;
        }
        return false;
    }
    public bool Position_at_Corner()
    {
        List<Tile_Controller> cornerTiles = mapController.combinationSystem.Map_Corners();

        for (int i = 0; i < cornerTiles.Count; i++)
        {
            if (!cornerTiles[i].Has_Prefab_ID(Prefab_Type.character, 0)) continue;
            return true;
        }
        return false;
    }

    // updates
    public void Set_Data(TileMap_Controller mapController)
    {
        this.mapController = mapController;
    }
    public void Update_Position(int row, int column)
    {
        currentRowNum = row;
        currentColumnNum = column;
    }

    // functions
    public void Click()
    {
        if (!interactReady) 
        {
            interactReady = true;
            mapController.actionSystem.Highlight_Player_Moveable_Tiles();
            mapController.actionSystem.Set_NewMap_Directions();
        }
        else
        {
            interactReady = false;
            mapController.actionSystem.UnHighlight_All_tiles();
            mapController.actionSystem.Reset_NewMap_Directions();
        }
    }

    public void Move()
    {
        LeanTween.moveLocal(gameObject, Vector2.zero, moveSpeed).setEase(LeanTweenType.easeInOutQuint);
        interactReady = false;
    }
}
