using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap_Action_System : MonoBehaviour
{
    private TileMap_Controller _mapController;
    public TileMap_Controller mapController { get => _mapController; set => _mapController = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Controller mapController)) { this.mapController = mapController; }
    }

    public void UnHighlight_All_tiles()
    {
        List<Tile_Controller> tiles = mapController.tiles;

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].UnHighlight_Tile();
        }
    }

    public void Highlight_All_Moveable_Tiles()
    {
        List<Tile_Controller> tiles = mapController.tiles;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].Has_Prefab(Prefab_Type.all)) continue;
            tiles[i].Highlight_Tile();
        }
    }

    public void Highlight_Player_Moveable_Tiles()
    {
        List<Tile_Controller> moveableTiles = mapController.combinationSystem.Cross_Tiles(Prefab_Type.character, 0);

        for (int i = 0; i < moveableTiles.Count; i++)
        {
            if (moveableTiles[i].Has_Prefab(Prefab_Type.placeable)) continue;
            moveableTiles[i].Highlight_Tile();
        }
    }
}
