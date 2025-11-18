using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static Vector2 SnapPosition(Vector2 position)
    {
        float xPos = Mathf.RoundToInt(position.x);
        float yPos = Mathf.RoundToInt(position.y);
        
        return new Vector2(xPos, yPos);
    }
    
    public static List<Vector2> SurroundingPositions(Vector2 position)
    {
        return new List<Vector2>
        {
            position + Vector2.up,
            position + new Vector2(1, 1),
            position + Vector2.right,
            position + new Vector2(1, -1),
            position + Vector2.down,
            position + new Vector2(-1, -1),
            position + Vector2.left,
            position + new Vector2(-1, 1),
        };
    }

    public static List<Vector2> ClosestConverted_Positions(Vector2 pivotPosition, List<Vector2> positionsToConvert)
    {
        List<Vector2> convertedPositions = new(positionsToConvert);

        convertedPositions.Sort((a, b) =>
        {
            float distA = Vector2.Distance(a, pivotPosition);
            float distB = Vector2.Distance(b, pivotPosition);
            return distA.CompareTo(distB);
        });

        return convertedPositions;
    }
}
