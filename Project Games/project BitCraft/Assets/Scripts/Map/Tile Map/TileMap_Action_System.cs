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
            tiles[i].Highlight();
        }
    }

    // player action systems
    public void Highlight_Player_Moveable_Tiles()
    {
        List<Tile_Controller> moveableTiles = mapController.combinationSystem.Cross_Tiles(Prefab_Type.character, 0);

        for (int i = 0; i < moveableTiles.Count; i++)
        {
            if (moveableTiles[i].Is_Prefab_Type(Prefab_Type.placeable) || moveableTiles[i].Has_Prefab_Type(Prefab_Type.placeable))
            {
                moveableTiles.RemoveAt(i);
                i--;

                if (moveableTiles.Count <= 0)
                {
                    mapController.playerController.interactReady = false;
                    break;
                }

                continue;
            }

            moveableTiles[i].Highlight();
        }
    }

    public void Set_NewMap_Directions()
    {
        Player_Controller player = mapController.playerController;
        Tile_Controller playerTile = mapController.Get_Tile(player.currentRowNum, player.currentColumnNum);

        playerTile.directionSystem.Set_Directions();
    }
    public void Reset_NewMap_Directions()
    {
        Player_Controller player = mapController.playerController;
        Tile_Controller playerTile = mapController.Get_Tile(player.currentRowNum, player.currentColumnNum);

        playerTile.directionSystem.Reset_Directions();
    }

    public void Move_Player(Tile_Controller moveTile)
    {
        Player_Controller player = mapController.playerController;

        moveTile.Set_Prefab(player.transform);

        player.Update_Tile_Position(moveTile.rowNum, moveTile.columnNum);
        player.Move();

        mapController.AllTiles_Update_Data();
    }
}