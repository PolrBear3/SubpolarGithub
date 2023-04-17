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
        List<Tile_Controller> tiles = mapController.tiles;

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].UnHighlight();
        }
    }
    public void Highlight_All_Moveable_Tiles()
    {
        List<Tile_Controller> tiles = mapController.tiles;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].Has_Prefab_Type(Prefab_Type.all)) continue;
            tiles[i].Highlight();
        }
    }

    // player action systems
    public void Highlight_Player_Moveable_Tiles()
    {
        List<Tile_Controller> moveableTiles = mapController.combinationSystem.Prefab_Cross(Prefab_Type.character, 0);

        if (moveableTiles.Count <= 0)
        {
            mapController.playerController.moveReady = false;
            return;
        }

        for (int i = 0; i < moveableTiles.Count; i++)
        {
            // if tile type is not placeable
            if (moveableTiles[i].Is_Prefab_Type(Prefab_Type.placeable)) continue;
            // if tile doesnt have placeable prefab
            if (moveableTiles[i].Has_Prefab_Type(Prefab_Type.placeable)) continue;

            moveableTiles[i].Highlight();
        }
    }
    public void Move_Player(Tile_Controller moveTile)
    {
        Player_Controller player = mapController.playerController;

        player.transform.parent = moveTile.transform;
        mapController.AllTiles_Update_Data();
        player.Update_Position(moveTile.rowNum, moveTile.columnNum);
        player.Move();
    }
}