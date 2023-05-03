using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap_Combination_System : MonoBehaviour
{
    private TileMap_Controller _mapController;
    public TileMap_Controller mapController { get => _mapController; set => _mapController = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Controller mapController)) { this.mapController = mapController; }
    }

    private void Remove_Null_Tiles(List<Tile_Controller> tileList)
    {
        for (int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i] != null) continue;
            tileList.RemoveAt(i);
            i--;
        }
    }

    public List<Tile_Controller> Cross_Tiles(int rowNum, int columnNum)
    {
        List<Tile_Controller> crossTiles = new List<Tile_Controller>();
        Tile_Controller mainTile = mapController.Get_Tile(rowNum, columnNum);

        // left tile
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum - 1, mainTile.columnNum));
        // right tile
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum + 1, mainTile.columnNum));
        // top tile
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum, mainTile.columnNum - 1));
        // bottom tile
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum, mainTile.columnNum + 1));

        Remove_Null_Tiles(crossTiles);

        return crossTiles;
    }
    public List<Tile_Controller> Cross_Tiles(Prefab_Type type, int prefabID)
    {
        List<Tile_Controller> crossTiles = new List<Tile_Controller>();
        Tile_Controller mainTile = mapController.Get_Tile(type, prefabID);

        // left tile
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum - 1, mainTile.columnNum));
        // right tile
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum + 1, mainTile.columnNum));
        // top tile
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum, mainTile.columnNum - 1));
        // bottom tile
        crossTiles.Add(mapController.Get_Tile(mainTile.rowNum, mainTile.columnNum + 1));

        Remove_Null_Tiles(crossTiles);

        return crossTiles;
    }

    public List<Tile_Controller> Surrounding_Tiles(Tile_Controller targetTile)
    {
        List<Tile_Controller> allTiles = mapController.currentMap.tiles;
        List<Tile_Controller> surroundingTiles = new List<Tile_Controller>();

        for (int i = 0; i < allTiles.Count; i++)
        {
            // if tile row number is not 1 and 2 and 3, skip
            if (allTiles[i].rowNum != targetTile.rowNum - 1 && allTiles[i].rowNum != targetTile.rowNum && allTiles[i].rowNum != targetTile.rowNum + 1) continue;
            // if tile column number is not 1 and 2 and 3, skip
            if (allTiles[i].columnNum != targetTile.columnNum - 1 && allTiles[i].columnNum != targetTile.columnNum && allTiles[i].columnNum != targetTile.columnNum + 1) continue;

            surroundingTiles.Add(allTiles[i]);
        }

        return surroundingTiles;
    }
    public List<Tile_Controller> Surrounding_Tiles(int rowNum, int columnNum)
    {
        List<Tile_Controller> allTiles = mapController.currentMap.tiles;
        List<Tile_Controller> surroundingTiles = new List<Tile_Controller>();
        Tile_Controller targetTile = mapController.Get_Tile(rowNum, columnNum);

        for (int i = 0; i < allTiles.Count; i++)
        {
            // if tile row number is not 1 and 2 and 3, skip
            if (allTiles[i].rowNum != targetTile.rowNum - 1 && allTiles[i].rowNum != targetTile.rowNum && allTiles[i].rowNum != targetTile.rowNum + 1) continue;
            // if tile column number is not 1 and 2 and 3, skip
            if (allTiles[i].columnNum != targetTile.columnNum - 1 && allTiles[i].columnNum != targetTile.columnNum && allTiles[i].columnNum != targetTile.columnNum + 1) continue;

            surroundingTiles.Add(allTiles[i]);
        }

        return surroundingTiles;
    }
    public List<Tile_Controller> Surrounding_Tiles(Prefab_Type type, int prefabID)
    {
        List<Tile_Controller> allTiles = mapController.currentMap.tiles;
        List<Tile_Controller> surroundingTiles = new List<Tile_Controller>();
        Tile_Controller prefabTile = mapController.Get_Tile(type, prefabID);

        for (int i = 0; i < allTiles.Count; i++)
        {
            // if tile row number is not 1 and 2 and 3, skip
            if (allTiles[i].rowNum != prefabTile.rowNum - 1 && allTiles[i].rowNum != prefabTile.rowNum && allTiles[i].rowNum != prefabTile.rowNum + 1) continue;
            // if tile column number is not 1 and 2 and 3, skip
            if (allTiles[i].columnNum != prefabTile.columnNum - 1 && allTiles[i].columnNum != prefabTile.columnNum && allTiles[i].columnNum != prefabTile.columnNum + 1) continue;

            surroundingTiles.Add(allTiles[i]);
        }

        return surroundingTiles;
    }

    public List<Tile_Controller> Map_Crust_Tiles()
    {
        List<Tile_Controller> allTiles = mapController.currentMap.tiles;
        List<Tile_Controller> crustTiles = new List<Tile_Controller>();

        for (int i = 0; i < allTiles.Count; i++)
        {
            // not left top tiles
            if (allTiles[i].rowNum != 0 && allTiles[i].columnNum != 0)
            {
                // not right bottom tiles
                if (allTiles[i].rowNum != mapController.currentMap.mapSize - 1 && allTiles[i].columnNum != mapController.currentMap.mapSize - 1) continue;
            }

            crustTiles.Add(allTiles[i]);
        }

        return crustTiles;
    }
    public List<Tile_Controller> Map_Corners_Tiles()
    {
        List<Tile_Controller> allTiles = mapController.currentMap.tiles;
        List<Tile_Controller> cornerTiles = new List<Tile_Controller>();

        for (int i = 0; i < allTiles.Count; i++)
        {
            // top corners
            if (allTiles[i].rowNum == 0 || allTiles[i].rowNum == mapController.currentMap.mapSize - 1)
            {
                // bottom corners
                if (allTiles[i].columnNum == 0 || allTiles[i].columnNum == mapController.currentMap.mapSize - 1)
                {
                    cornerTiles.Add(allTiles[i]);
                }
            }
        }

        return cornerTiles;
    }
}