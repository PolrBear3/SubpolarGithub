using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell_Controller : MonoBehaviour
{
    private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;

    private GridCell _data;
    public GridCell data => _data;



    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }


    // Data
    public GridCell Set_Data(GridCell setData)
    {
        _data = setData;
        return _data;
    }


    // Visual
    public void Toggle_Transparency(bool toggle)
    {
        if (toggle)
        {
            _sr.color = Color.clear;
            return;
        }

        _sr.color = Color.white;
    }
}
