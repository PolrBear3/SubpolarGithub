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

    // basic tile control
    private void Remove_Tiles()
    {
        for (int i = 0; i < mapController.tiles.Count; i++)
        {
            Destroy(mapController.tiles[i].gameObject);
        }

        mapController.tiles.Clear();
    }
    public void Set_Tiles(int size)
    {
        mapController.mapSize = size;

        int rowNum = 0;
        int columnNum = 0;

        for (int i = 0; i < size * size; i++)
        {
            // set position
            Vector2 position = new(rowNum - 2, -columnNum + 2);

            // instantiate
            GameObject tile = Instantiate(mapController.controller.prefabsController.Get_Random_Tile(), position, Quaternion.identity);

            // set inside tile map as child
            tile.transform.parent = transform;

            // get tile controller component
            if (tile.TryGetComponent(out Tile_Controller tileData))
            {
                // add to tiles list
                mapController.tiles.Add(tileData);

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

    // public tile control
    public void Render_Next_Map(bool isRowMove)
    {
        // save player position
        Player_Controller player = mapController.playerController;
        int previousRow = player.currentRowNum;
        int previousColumn = player.currentColumnNum;

        // save and destroy previous tiles
        Remove_Tiles();

        // set new tiles
        Set_Tiles(5);

        // set next player position
        if (isRowMove) previousRow += mapController.mapSize - 1;
        else previousColumn += mapController.mapSize - 1;

        // set player to left and top
        if (previousRow > mapController.mapSize - 1) { previousRow = 0; }
        else if (previousColumn > mapController.mapSize - 1) { previousColumn = 0; }

        // set player on map
        mapController.Set_Player(previousRow, previousColumn);
    }
}
