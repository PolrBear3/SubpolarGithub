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
        int mapSizeArray = currentMap.mapSize - 1;

        List<Tile_Controller> crossTiles = new List<Tile_Controller>(4)
        {
            mapController.Get_Tile(_tileController.rowNum, _tileController.columnNum - 1),
            mapController.Get_Tile(_tileController.rowNum + 1, _tileController.columnNum),
            mapController.Get_Tile(_tileController.rowNum , _tileController.columnNum + 1),
            mapController.Get_Tile(_tileController.rowNum - 1, _tileController.columnNum)
        };

        for (int i = 0; i < crossTiles.Count; i++)
        {
            if (crossTiles[i] != null) continue;

            int positionX = currentMap.positionX + (i == 1 ? 1 : i == 3 ? -1 : 0);
            int positionY = currentMap.positionY + (i == 0 ? 1 : i == 2 ? -1 : 0);
            Map_Controller surroundingMap = mapController.Get_Map(positionX, positionY);

            if (surroundingMap == null) continue;

            int rowNum = i == 1 ? 0 : i == 3 ? mapSizeArray : _tileController.rowNum;
            int column = i == 0 ? mapSizeArray : i == 2 ? 0 : _tileController.columnNum;
            crossTiles[i] = surroundingMap.Get_Tile(rowNum, column);
        }

        for (int i = 0; i < crossTiles.Count; i++)
        {
            if (crossTiles[i] == null || !crossTiles[i].Is_Prefab_Type(Prefab_Type.placeable))
            {
                _directions[i].SetActive(true);
            }
        }
    }
}
