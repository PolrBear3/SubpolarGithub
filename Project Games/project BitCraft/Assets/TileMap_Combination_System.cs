using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap_Combination_System : MonoBehaviour
{
    private TileMap_Controller mapController;

    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Controller mapController)) { this.mapController = mapController; }
    }

    public List<Tile_Controller> Cross_Tiles(Prefab_Type type, int prefabID)
    {
        List<Tile_Controller> allTiles = mapController.tiles;
        List<Tile_Controller> crossTiles = new List<Tile_Controller>();
        Tile_Controller mainTile = null;

        for (int i = 0; i < allTiles.Count; i++)
        {
            if (!allTiles[i].Has_Prefab_ID(type, prefabID)) continue;
            mainTile = allTiles[i];
            break;
        }

        // left tile 3 2
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum - 1, mainTile.columnNum));
        // right tile 5 2
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum + 1, mainTile.columnNum));
        // top tile 4 1
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum, mainTile.columnNum - 1));
        // bottom tile 4 3
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum, mainTile.columnNum + 1));

        for (int i = 0; i < crossTiles.Count; i++)
        {
            if (crossTiles[i] != null) continue;
            crossTiles.Remove(crossTiles[i]);
        }

        return crossTiles;
    }
}