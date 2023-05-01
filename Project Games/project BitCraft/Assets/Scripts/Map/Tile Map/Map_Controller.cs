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

    public void Update_Position(int positionX, int positionY)
    {
        this.positionX = positionX;
        this.positionY = positionY;
    }

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
