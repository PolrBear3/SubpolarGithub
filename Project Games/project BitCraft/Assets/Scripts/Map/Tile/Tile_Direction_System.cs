using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Direction_System : MonoBehaviour
{
    private Tile_Controller _tileController;

    [SerializeField] private GameObject[] _directions;

    [SerializeField] private List<Tile_Controller> crossTileEx = new List<Tile_Controller>();

    private void Awake()
    {
        if (transform.parent.gameObject.TryGetComponent(out Tile_Controller tileController))
        {
            _tileController = tileController;
            _tileController.directionSystem = this;
        }
    }

    public void Click(bool isRowMove)
    {
        _tileController.mapController.playerController.interactReady = false;
        _tileController.mapController.actionSystem.UnHighlight_All_tiles();
        Reset_Directions();

        _tileController.mapController.renderSystem.Render_Next_Map(isRowMove);
    }

    public void Reset_Directions()
    {
        for (int i = 0; i < _directions.Length; i++)
        {
            _directions[i].SetActive(false);
        }
    }
    public void Set_Directions()
    {
        TileMap_Controller mapController = _tileController.mapController;
        Map_Controller currentMap = mapController.currentMap;

        List<Tile_Controller> crossTiles = new List<Tile_Controller>();
        List<Map_Controller> surroundingMaps = new List<Map_Controller>();

        // top
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum, _tileController.columnNum - 1));
        surroundingMaps.Add(mapController.Get_Map(currentMap.positionX, currentMap.positionY++));
        if (crossTiles[0] == null && surroundingMaps[0] != null)
        {
            Tile_Controller targetTile = surroundingMaps[0].Get_Tile(currentMap.positionX, surroundingMaps[0].mapSize - 1);
            targetTile.gameObject.SetActive(true);
            crossTiles[0] = targetTile;
            targetTile.gameObject.SetActive(false);
        }

        // right
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum + 1, _tileController.columnNum));
        surroundingMaps.Add(mapController.Get_Map(currentMap.positionX++, currentMap.positionY));
        if (crossTiles[1] == null)
        {
            Debug.Log("check");
            Tile_Controller targetTile = surroundingMaps[1].Get_Tile(0, currentMap.positionY);
            targetTile.gameObject.SetActive(true);
            crossTiles[1] = targetTile;
            targetTile.gameObject.SetActive(false);
        }

        // bottom
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum , _tileController.columnNum + 1));
        surroundingMaps.Add(mapController.Get_Map(currentMap.positionX, currentMap.positionY--));
        if (crossTiles[2] == null && surroundingMaps[2] != null)
        {
            Tile_Controller targetTile = surroundingMaps[2].Get_Tile(currentMap.positionX, 0);
            targetTile.gameObject.SetActive(true);
            crossTiles[2] = targetTile;
            targetTile.gameObject.SetActive(false);
        }

        // left
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum - 1, _tileController.columnNum));
        surroundingMaps.Add(mapController.Get_Map(currentMap.positionX--, currentMap.positionY));
        if (crossTiles[3] == null && surroundingMaps[3] != null)
        {
            Tile_Controller targetTile = surroundingMaps[3].Get_Tile(surroundingMaps[3].mapSize - 1, currentMap.positionY);
            targetTile.gameObject.SetActive(true);
            crossTiles[3] = targetTile;
            targetTile.gameObject.SetActive(false);
        }

        crossTileEx = crossTiles;

        for (int i = 0; i < crossTiles.Count; i++)
        {
            if (crossTiles[i] != null) continue;

            _directions[i].SetActive(true);
        }
    }
}
