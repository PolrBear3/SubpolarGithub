using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Controller : MonoBehaviour
{
    private Game_Controller _gameController;
    public Game_Controller gameController { get => _gameController; set => _gameController = value; }

    [Header("Set Data")]
    [SerializeField] private List<Tile> _allTiles = new List<Tile>();
    public List<Tile> allTiles { get => _allTiles; set => _allTiles = value; }

    [SerializeField] private int levelSizeX;
    [SerializeField] private int levelSizeY;

    private void Awake()
    {
        Set_AllTiles_Data();
        AllGears_Spin_Activation_Check();
    }

    // Set
    private void Set_AllTiles_Data()
    {
        int xNum = 0;
        int yNum = 0;

        for (int i = 0; i < _allTiles.Count; i++)
        {
            _allTiles[i].Set_Data(this, xNum, yNum);
            xNum++;

            if (xNum <= levelSizeX) continue;
            yNum++;
            xNum = 0;
        }
    }

    // Get
    public Tile Get_Tile(int xNum, int yNum)
    {
        for (int i = 0; i < _allTiles.Count; i++)
        {
            if (_allTiles[i].xPosition != xNum) continue;
            if (_allTiles[i].yPosition != yNum) continue;
            return _allTiles[i];
        }
        return null;
    }
    public List<Tile> Surrounding_Tiles(Tile targetTile)
    {
        List<Tile> surroundingTiles = new List<Tile>();

        // top
        surroundingTiles.Add(Get_Tile(targetTile.xPosition, targetTile.yPosition - 1));
        // bottom
        surroundingTiles.Add(Get_Tile(targetTile.xPosition, targetTile.yPosition + 1));
        // left
        surroundingTiles.Add(Get_Tile(targetTile.xPosition - 1, targetTile.yPosition));
        // right
        surroundingTiles.Add(Get_Tile(targetTile.xPosition + 1, targetTile.yPosition));

        for (int i = surroundingTiles.Count - 1; i >= 0; i--)
        {
            if (surroundingTiles[i] != null) continue;
            surroundingTiles.RemoveAt(i);
        }

        return surroundingTiles;
    }

    // Update Check
    public void AllGears_Spin_Activation_Check()
    {
        for (int i = 0; i < _allTiles.Count; i++)
        {
            if (_allTiles[i].currentGear == null) continue;
            _allTiles[i].currentGear.Spin_Activation_Check();
        }
    }
}
