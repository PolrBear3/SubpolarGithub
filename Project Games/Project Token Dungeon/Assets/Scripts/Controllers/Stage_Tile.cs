using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Tile : MonoBehaviour
{
    public Location_Data data;
    public GameObject groundPoint;

    public void Set_Data(int rowNum, int columnNum)
    {
        data.rowNum = rowNum;
        data.columnNum = columnNum;
    }
}