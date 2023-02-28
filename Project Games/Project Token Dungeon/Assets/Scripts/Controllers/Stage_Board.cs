using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stage_Board_Data
{
    public int maxRowNum;
    public int maxColumnNum;
}

public class Stage_Board : MonoBehaviour
{
    [HideInInspector] public Game_Manager manager;

    [SerializeField] private Stage_Board_Data data;

    public List<Stage_Tile> tiles = new List<Stage_Tile>();
    public Stage_Tile startingTile;

    public void Connect_Manager(Game_Manager manager)
    {
        this.manager = manager;
    }
    public void Set_Tile_Data()
    {
        int rowNum = 0;
        int columnNum = 0;
        
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].Set_Data(rowNum, columnNum);

            columnNum++;

            if (columnNum != data.maxColumnNum) continue;

            columnNum = 0;
            rowNum++;
        }
    }

    public bool In_StageBoard(Location_Data data)
    {
        if (data.rowNum > this.data.maxRowNum || data.rowNum < 0) return false;
        if (data.columnNum > this.data.maxColumnNum || data.columnNum < 0) return false;

        return true;
    }
    public Stage_Tile Find_Tile(Location_Data data)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (data.rowNum != tiles[i].data.rowNum) continue;
            if (data.columnNum != tiles[i].data.columnNum) continue;

            return tiles[i];
        }

        return null;
    }
}