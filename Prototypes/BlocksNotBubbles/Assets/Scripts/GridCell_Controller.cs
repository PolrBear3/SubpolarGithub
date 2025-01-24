using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell_Controller : MonoBehaviour
{
    private GridCell _data;
    public GridCell data => _data;


    // Data
    public GridCell Set_Data(GridCell setData)
    {
        _data = setData;
        return _data;
    }
}
