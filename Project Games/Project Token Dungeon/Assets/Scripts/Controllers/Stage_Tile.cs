using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stage_Tile_Data
{
    public int rowNum;
    public int columnNum;
}

public class Stage_Tile : MonoBehaviour
{
    [SerializeField] private Stage_Tile_Data data;
    public GameObject groundPoint;

    public void Set_Data(int rowNum, int columnNum)
    {
        data.rowNum = rowNum;
        data.columnNum = columnNum;
    }
}