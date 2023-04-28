using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap_Render_System : MonoBehaviour
{
    private TileMap_Controller _mapController;
    public TileMap_Controller mapController { get => _mapController; set => _mapController = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Controller mapController)) { this.mapController = mapController; }
    }

    private void Set_New_Tiles(int size)
    {
        mapController.currentMap.mapSize = size;

        int rowNum = 0;
        int columnNum = 0;

        for (int i = 0; i < size * size; i++)
        {
            // set position
            Vector2 position = new(rowNum - 2, -columnNum + 2);

            // instantiate
            GameObject tile = Instantiate(mapController.controller.prefabsController.Get_Random_Tile(), position, Quaternion.identity);

            // set inside tile map as child
            tile.transform.parent = mapController.currentMap.transform;

            // get tile controller component
            if (tile.TryGetComponent(out Tile_Controller tileData))
            {
                // add to tiles list
                mapController.currentMap.tiles.Add(tileData);

                // set tile data
                tileData.Set_Data(mapController);

                // update tile position
                tileData.Update_Position(rowNum, columnNum);
            }

            // tile num update
            rowNum++;

            if (rowNum <= 4) continue;

            rowNum = 0;
            columnNum++;
        }
    }
    public void Set_New_Map(int mapSize)
    {
        // spawn map prefab
        GameObject setMap = Instantiate(this.mapController.controller.prefabsController.Get_MapController());

        // set map inside tilemap controller as child
        setMap.transform.parent = transform;

        // connect to current map component
        if (!setMap.TryGetComponent(out Map_Controller mapController)) return;
        this.mapController.currentMap = mapController;

        // add new map to all map list
        this.mapController.allMaps.Add(this.mapController.currentMap);

        // set tiles inside current map
        Set_New_Tiles(mapSize);
    }

    public void Render_Next_Map(bool isRowMove)
    {
        // save player position
        Player_Controller player = mapController.playerController;
        int previousRow = player.currentRowNum;
        int previousColumn = player.currentColumnNum;

        // move current map to next position and hide
        mapController.currentMap.Save_Map();

        // set new map
        Set_New_Map(5);

        // save map size
        int mapSize = mapController.currentMap.mapSize - 1;

        // set next player position
        if (isRowMove) previousRow += mapSize;
        else previousColumn += mapSize;

        // set player to left and top
        if (previousRow > mapSize) { previousRow = 0; }
        else if (previousColumn > mapSize) { previousColumn = 0; }

        // set player on map
        mapController.Set_Player(previousRow, previousColumn);
    }
}
