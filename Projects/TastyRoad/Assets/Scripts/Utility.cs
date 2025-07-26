using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public struct UnityEvent_Data
{
    [Range(0, 100)] public float probability;
    public UnityEvent action;
}

public static class Utility
{
    /// <summary>
    /// Converts position x and y to nearest integer
    /// </summary>
    public static Vector2 SnapPosition(Vector2 position)
    {
        float snapX = (float)Mathf.Round(position.x);
        float snapY = (float)Mathf.Round(position.y);

        return new Vector2(snapX, snapY);
    }
    
    /// <returns>
    /// Converts position x and y to nearest integer within bounds
    /// </returns>
    public static Vector2 SnapPosition(Vector2 position, Bounds bounds)
    {
        // Round the position to the nearest integer
        int snapX = Mathf.RoundToInt(position.x);
        int snapY = Mathf.RoundToInt(position.y);

        // Clamp the snapped position within the integer bounds
        snapX = Mathf.Clamp(snapX, Mathf.CeilToInt(bounds.min.x), Mathf.FloorToInt(bounds.max.x));
        snapY = Mathf.Clamp(snapY, Mathf.CeilToInt(bounds.min.y), Mathf.FloorToInt(bounds.max.y));

        return new Vector2(snapX, snapY);
    }
    
    /// <returns>
    /// all 8 surrounding positions from centerPosition
    /// (Order: Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight)
    /// </returns>
    public static List<Vector2> Surrounding_SnapPositions(Vector2 centerPosition)
    {
        return new List<Vector2>
        {
            SnapPosition(centerPosition + Vector2.up),
            SnapPosition(centerPosition + Vector2.down),
            SnapPosition(centerPosition + Vector2.left),
            SnapPosition(centerPosition + Vector2.right),

            SnapPosition(centerPosition + new Vector2(-1, 1)),
            SnapPosition(centerPosition + new Vector2(1, 1)),
            SnapPosition(centerPosition + new Vector2(-1, -1)),
            SnapPosition(centerPosition + new Vector2(1, -1))
        };
    }
    
    /// <returns>
    /// +1 and -1 of x and y position from centerPosition
    /// </returns>
    public static List<Vector2> CrossSurrounding_SnapPositions(Vector2 centerPosition)
    {
        return new List<Vector2>
        {
            SnapPosition(centerPosition + Vector2.up),
            SnapPosition(centerPosition + Vector2.down),
            SnapPosition(centerPosition + Vector2.left),
            SnapPosition(centerPosition + Vector2.right),
        };
    }
    
    /// <returns>
    /// random point inside bound
    /// </returns>
    public static Vector2 Random_BoundPoint(Bounds bound)
    {
        Vector2 centerPosition = bound.center;

        float randX = UnityEngine.Random.Range(centerPosition.x - bound.extents.x, centerPosition.x + bound.extents.x);
        float randY = UnityEngine.Random.Range(centerPosition.y - bound.extents.y, centerPosition.y + bound.extents.y);

        return new Vector2(randX, randY);
    }
    
    
    /// <summary>
    /// Checks if percentage is more than a random value
    /// </summary>
    public static bool Percentage_Activated(float percentage)
    {
        float comparePercentage = Mathf.Round(UnityEngine.Random.Range(0f, 100f)) * 1f;
        return percentage >= comparePercentage;
    }
    
    
    /// <summary>
    /// Changes inserted SpriteRenderer to target transparency
    /// </summary>
    public static void Change_SpriteAlpha(SpriteRenderer sr, float alpha)
    {
        if (alpha > 1f) alpha = Mathf.Clamp01(alpha / 100f);
        
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }

    /// <summary>
    /// Changes inserted Image to target transparency
    /// </summary>
    public static void Change_ImageAlpha(Image image, float alpha)
    {
        if (alpha > 1f) alpha = Mathf.Clamp01(alpha / 100f);
        
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
