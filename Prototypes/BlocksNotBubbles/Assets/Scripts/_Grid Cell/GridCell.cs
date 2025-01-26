using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridCell
{
    private Vector2 _position;
    public Vector2 position => _position;

    private List<Vector2> _surroundingPos = new();
    public List<Vector2> surroundingPos => _surroundingPos;

    private bool _occupied;
    public bool occupied => _occupied;

    private Block_Controller _occupiedBlock;
    public Block_Controller occupiedBlock => _occupiedBlock;


    // GridCell
    public GridCell(Vector2 position)
    {
        _position = position;
    }


    // Control
    public void Toggle_Occupation(bool toggle)
    {
        _occupied = toggle;
    }

    public void Update_Block(Block_Controller updateBlock)
    {
        if (updateBlock == null)
        {
            _occupiedBlock = null;

            Toggle_Occupation(false);
            return;
        }

        _occupiedBlock = updateBlock;

        Toggle_Occupation(true);
    }
}
