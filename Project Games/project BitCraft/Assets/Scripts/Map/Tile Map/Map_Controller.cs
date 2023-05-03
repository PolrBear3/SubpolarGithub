using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Controller : MonoBehaviour
{
    [SerializeField] private int _positionX;
    public int positionX { get => _positionX; set => _positionX = value; }

    [SerializeField] private int _positionY;
    public int positionY { get => _positionY; set => _positionY = value; }

    [SerializeField] private int _mapSize;
    public int mapSize { get => _mapSize; set => _mapSize = value; }

    [SerializeField] private List<Tile_Controller> _tiles = new List<Tile_Controller>();
    public List<Tile_Controller> tiles { get => _tiles; set => _tiles = value; }

    // tile data
    public Tile_Controller Get_Tile(int rowNum, int columnNum)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (!tiles[i].Found(rowNum, columnNum)) continue;
            return tiles[i];
        }
        return null;
    }
    public Tile_Controller Get_Tile(Prefab_Type type, int prefabID)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (!tiles[i].Has_Prefab_ID(type, prefabID)) continue;
            return tiles[i];
        }
        return null;
    }

    // update data
    public void Update_Position(int positionX, int positionY)
    {
        this.positionX = positionX;
        this.positionY = positionY;
    }

    // public functions
    public void Save_Hide_Map()
    {
        // hide all tiles
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].gameObject.SetActive(false);
        }
    }
    public void Load_Show_Map()
    {
        // show all tiles
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].gameObject.SetActive(true);
        }
    }
}
