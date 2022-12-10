using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileKeySelection_Controller : MonoBehaviour
{
    public TileKeySelection_Tile[] tiles;
    public TileKeySelection_Tile currentTile;

    public int currentTileNum;

    private void Start()
    {
        Set_CurrentTile();
        tiles[currentTileNum].Select();
    }

    private void Set_CurrentTile()
    {
        currentTile = tiles[currentTileNum];
    }
    private void Max_Min_TileNum_Check()
    {
        if (currentTileNum < 0)
        {
            currentTileNum += tiles.Length;
        }
        else if (currentTileNum > tiles.Length - 1)
        {
            currentTileNum -= tiles.Length;
        }
    }

    public void OnNext()
    {
        tiles[currentTileNum].UnSelect();
        currentTileNum++;
        Max_Min_TileNum_Check();
        Set_CurrentTile();
        tiles[currentTileNum].Select();
    }
    public void OnBack()
    {
        tiles[currentTileNum].UnSelect();
        currentTileNum--;
        Max_Min_TileNum_Check();
        Set_CurrentTile();
        tiles[currentTileNum].Select();
    }
    public void OnDown()
    {
        tiles[currentTileNum].UnSelect();
        currentTileNum += 5;
        Max_Min_TileNum_Check();
        Set_CurrentTile();
        tiles[currentTileNum].Select();
    }
    public void OnUp()
    {
        tiles[currentTileNum].UnSelect();
        currentTileNum -= 5;
        Max_Min_TileNum_Check();
        Set_CurrentTile();
        tiles[currentTileNum].Select();
    }
}
