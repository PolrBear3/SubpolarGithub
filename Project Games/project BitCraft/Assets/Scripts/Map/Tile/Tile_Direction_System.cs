using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Direction_System : MonoBehaviour
{
    private Tile_Controller _tileController;
    
    [SerializeField] private GameObject[] _directions;

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

        // top
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum, _tileController.columnNum - 1));
        Map_Controller topMap = mapController.Get_Map(currentMap.positionX, currentMap.positionY + 1);
        if (crossTiles[0] == null && topMap != null) crossTiles[0] = topMap.Get_Tile(_tileController.rowNum, currentMap.mapSize - 1);

        // right
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum + 1, _tileController.columnNum));
        Map_Controller rightMap = mapController.Get_Map(currentMap.positionX + 1, currentMap.positionY);
        if (crossTiles[1] == null && rightMap != null) crossTiles[1] = rightMap.Get_Tile(0, _tileController.columnNum);

        // bottom
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum , _tileController.columnNum + 1));
        Map_Controller bottomMap = mapController.Get_Map(currentMap.positionX, currentMap.positionY - 1);
        if (crossTiles[2] == null && bottomMap != null) crossTiles[2] = bottomMap.Get_Tile(_tileController.rowNum, 0);

        // left
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum - 1, _tileController.columnNum));
        Map_Controller leftMap = mapController.Get_Map(currentMap.positionX - 1, currentMap.positionY);
        if (crossTiles[3] == null && leftMap != null) crossTiles[3] = leftMap.Get_Tile(currentMap.mapSize - 1, _tileController.columnNum);

        for (int i = 0; i < crossTiles.Count; i++)
        {
            if (crossTiles[i] == null || !crossTiles[i].Is_Prefab_Type(Prefab_Type.placeable))
            {
                _directions[i].SetActive(true);
            }
        }
    }
}
