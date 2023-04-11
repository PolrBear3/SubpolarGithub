using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap_Action_System : MonoBehaviour
{
    private TileMap_Controller mapController;

    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Controller mapController)) { this.mapController = mapController; }
    }

    private void Highlight_Moveable_Tiles()
    {
        List<Tile_Controller> tiles = mapController.tiles;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].Has_Prefab()) continue;

            tiles[i].Highlight_Tile();
        }
    }

    private void Highlight_Player_Moveable_Tiles()
    {
        // set available tiles for player
        List<Tile_Controller> moveableTiles = mapController.combinationSystem.Cross_Tiles(0);

        // check if it has other prefabs
        for (int i = 0; i < moveableTiles.Count; i++)
        {
            if (moveableTiles[i].Has_Prefab()) continue;

            moveableTiles[i].Highlight_Tile();
        }
    }
}
