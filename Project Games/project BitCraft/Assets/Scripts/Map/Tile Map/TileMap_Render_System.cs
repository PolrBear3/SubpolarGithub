using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap_Render_System : MonoBehaviour
{
    private TileMap_Controller _tilemapcontroller;
    public TileMap_Controller tilemapController { get => _tilemapcontroller; set => _tilemapcontroller = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Controller mapController)) { tilemapController = mapController; }
    }

    private void Set_New_Tiles(int size)
    {
        tilemapController.currentMap.mapSize = size;

        int rowNum = 0;
        int columnNum = 0;

        for (int i = 0; i < size * size; i++)
        {
            // set position
            Vector2 position = new(rowNum - 2, -columnNum + 2);

            // instantiate
            GameObject tile = Instantiate(tilemapController.controller.prefabsData.Get_Random_Tile(), position, Quaternion.identity);

            // set inside tile map as child
            tile.transform.parent = tilemapController.currentMap.transform;

            // get tile controller component
            if (tile.TryGetComponent(out Tile_Controller tileData))
            {
                // add to tiles list
                tilemapController.currentMap.tiles.Add(tileData);

                // set tile data
                tileData.Set_Data(tilemapController);

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
        GameObject setMap = Instantiate(tilemapController.controller.prefabsData.Get_MapController());

        // set map inside tilemap controller as child
        setMap.transform.parent = transform;

        // connect to current map component
        if (!setMap.TryGetComponent(out Map_Controller mapController)) return;
        tilemapController.currentMap = mapController;

        // add new map to all map list
        tilemapController.allMaps.Add(tilemapController.currentMap);

        // set tiles inside current map
        Set_New_Tiles(mapSize);
    }

    public void Render_Next_Map(bool isRowMove)
    {
        // save previous map
        Map_Controller previousMap = tilemapController.currentMap;

        // save player position
        Prefab_Controller playerPrefabController = tilemapController.playerPrefabController;
        int previousRow = playerPrefabController.currentRowNum;
        int previousColumn = playerPrefabController.currentColumnNum;

        // save map position
        int previousPosX = previousMap.positionX;
        int previousPosY = previousMap.positionY;

        // save map size
        int mapSize = tilemapController.currentMap.mapSize - 1;

        // remove player from previous map
        Tile_Controller playerTile = tilemapController.Get_Tile(previousRow, previousColumn);
        playerTile.Remove_Prefab(Prefab_Type.character, 0);

        // save and hide previous map
        tilemapController.currentMap.Map_Activation(false);

        // determine if row or column
        if (isRowMove) previousRow += mapSize;
        else previousColumn += mapSize;

        // set player to left (right map move)
        if (isRowMove && previousRow > mapSize) { previousRow = 0; previousPosX++; }
        // set player to right (left map move)
        else if (isRowMove) previousPosX--;

        // set player to bottom (top map move)
        if (!isRowMove && previousColumn > mapSize) { previousColumn = 0; previousPosY--; }
        // set player to top (bottom map move)
        else if (!isRowMove) previousPosY++;

        // check world size for map position
        int worldSize = tilemapController.worldSize;

        if (previousPosX > worldSize / 2) previousPosX = -(previousPosX - 1);
        else if (previousPosX < -(worldSize / 2)) previousPosX = -(previousPosX + 1);

        if (previousPosY > worldSize / 2) previousPosY = -(previousPosY - 1);
        else if (previousPosY < -(worldSize / 2)) previousPosY = -(previousPosY + 1);

        // new map render check
        bool hasMap = tilemapController.Has_Map(previousPosX, previousPosY);

        if (hasMap)
        {
            Map_Controller loadMap = tilemapController.Get_Map(previousPosX, previousPosY);
            loadMap.Map_Activation(true);
            tilemapController.currentMap = loadMap;
        }
        else
        {
            Set_New_Map(5);
            tilemapController.currentMap.Update_Position(previousPosX, previousPosY);
        }

        // set player on map
        tilemapController.Set_Character(0, previousRow, previousColumn);

        // update player map position
        Map_Controller currentMap = tilemapController.currentMap;
        playerPrefabController.Update_Map_Position(currentMap.positionX, currentMap.positionY);

        // set player tile
        if (!hasMap) tilemapController.Set_Player_Tile(false);
    }
}
