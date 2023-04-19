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

    public void Next_Map_Check_Update()
    {
        Player_Controller player = mapController.playerController;

        if (!player.Position_at_Crust()) return;

        List<Vector2> emptyPositions = mapController.Get_Tile(player.currentRowNum, player.currentColumnNum).Empty_Positions();

        for (int i = 0; i < emptyPositions.Count; i++)
        {
            // example tile instantiation as basic tile
            GameObject nextTile = Instantiate(mapController.controller.prefabsController.Get_Tile(0), emptyPositions[i], Quaternion.identity);

            // set inside tile map as child
            nextTile.transform.parent = transform;
        }
    }
}
