using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Controller : MonoBehaviour
{
    private int _rowNum;
    public int rowNum { get => _rowNum; set => _rowNum = value; }

    private int _columnNum;
    public int columnNum { get => _columnNum; set => _columnNum = value; }

    private List<GameObject> _currentPrefabs = new List<GameObject>();
    public List<GameObject> currentPrefabs { get => _currentPrefabs; set => _currentPrefabs = value; }

    public bool Found(int rowNum, int columnNum)
    {
        if (rowNum != this.rowNum) return false;
        if (columnNum != this.columnNum) return false;
        return true;
    }

    public void Set_Data(int row, int column)
    {
        rowNum = row;
        columnNum = column;
    }
    public void Track_Current_Prefabs(Transform prefabTransform)
    {
        prefabTransform.parent = transform;

        currentPrefabs.Clear();

        if (transform.childCount <= 0) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            currentPrefabs.Add(transform.GetChild(i).gameObject);
        }
    }
}