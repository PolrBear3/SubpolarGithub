using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap_Action_System : MonoBehaviour
{
    private TileMap_Controller _mapController;
    public TileMap_Controller mapController { get => _mapController; set => _mapController = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Controller mapController)) { this.mapController = mapController; }
    }

    // public action systems
    public void UnHighlight_All_tiles()
    {
        List<Tile_Controller> tiles = mapController.currentMap.tiles;

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].UnHighlight();
        }
    }
    public void Highlight_All_Moveable_Tiles()
    {
        List<Tile_Controller> tiles = mapController.currentMap.tiles;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].Has_Prefab_Type(Prefab_Type.all)) continue;
            tiles[i].Highlight(Color.green);
        }
    }

    // player action systems
    public void Highlight_Player_Interactable_Tiles()
    {
        List<Tile_Controller> crossTiles = mapController.combinationSystem.Cross_Tiles(Prefab_Type.character, 0);

    }

    public void Set_NewMap_Directions()
    {
        Prefab_Controller playerPrefabController = mapController.playerPrefabController;
        Tile_Controller playerTile = mapController.Get_Tile(playerPrefabController.currentRowNum, playerPrefabController.currentColumnNum);

        playerTile.directionSystem.Set_Directions();
    }
    public void Reset_NewMap_Directions()
    {
        Prefab_Controller playerPrefabController = mapController.playerPrefabController;
        Tile_Controller playerTile = mapController.Get_Tile(playerPrefabController.currentRowNum, playerPrefabController.currentColumnNum);

        playerTile.directionSystem.Reset_Directions();
    }

    public void Move_Player(Tile_Controller moveTile)
    {
        Player_Controller playerController = mapController.playerController;
        Prefab_Controller playerPrefabController = mapController.playerPrefabController;

        moveTile.Set_Prefab(playerController.transform);

        playerPrefabController.Update_Tile_Position(moveTile.rowNum, moveTile.columnNum);
        playerController.Move();

        mapController.AllTiles_Update_Data();
    }
}