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
        // player interact ready, default state
        _tileController.mapController.playerController.Click();

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
        List<Tile_Controller> crossTiles = new List<Tile_Controller>();

        // top
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum, _tileController.columnNum - 1));
        // right
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum + 1, _tileController.columnNum));
        // bottom
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum , _tileController.columnNum + 1));
        // left
        crossTiles.Add(mapController.Get_Tile(_tileController.rowNum - 1, _tileController.columnNum));

        for (int i = 0; i < crossTiles.Count; i++)
        {
            if (crossTiles[i] != null) continue;
            _directions[i].SetActive(true);
        }
    }
}
