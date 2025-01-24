using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridCell
{
    private int _xPos;
    public int xPos => _xPos;

    private int _yPos;
    public int yPos => _yPos;

    private bool _occupied;
    public bool occupied => _occupied;


    // GridCell
    public GridCell(int xPos, int yPos)
    {
        _xPos = xPos;
        _yPos = yPos;
    }
}
