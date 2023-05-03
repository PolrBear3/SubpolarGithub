using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Direction_System : MonoBehaviour
{
    private Tile_Controller _tileController;

    [SerializeField] private GameObject[] _directions;

    [SerializeField] private List<Tile_Controller> crossTileEx = new List<Tile_Controller>();
    [SerializeField] private List<Map_Controller> surroundingMapsEx = new List<Map_Controller>();

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
        if (crossTiles[0] == null && surroundingMaps[0] != null) crossTiles[0] = surroundingMaps[0].Get_Tile(currentMap.positionX, surroundingMaps[0].mapSize - 1);

        // right
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum + 1, _tileController.columnNum));
        surroundingMaps.Add(mapController.Get_Map(currentMap.positionX++, currentMap.positionY));
        if (crossTiles[1] == null && surroundingMaps[1] != null) crossTiles[1] = surroundingMaps[1].Get_Tile(0, currentMap.positionY);

        // bottom
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum , _tileController.columnNum + 1));
        surroundingMaps.Add(mapController.Get_Map(currentMap.positionX, currentMap.positionY--));
        if (crossTiles[2] == null && surroundingMaps[2] != null) crossTiles[2] = surroundingMaps[2].Get_Tile(currentMap.positionX, 0);

        // left
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum - 1, _tileController.columnNum));
        surroundingMaps.Add(mapController.Get_Map(currentMap.positionX--, currentMap.positionY));
        if (crossTiles[3] == null && surroundingMaps[3] != null) crossTiles[3] = surroundingMaps[3].Get_Tile(surroundingMaps[3].mapSize - 1, currentMap.positionY);

        crossTileEx = crossTiles;
        surroundingMapsEx = surroundingMaps;

        for (int i = 0; i < crossTiles.Count; i++)
        {
            if (crossTiles[i] != null) continue;
            if (crossTiles[i].Is_Prefab_Type(Prefab_Type.placeable)) continue;

            _directions[i].SetActive(true);
        }
    }
}
