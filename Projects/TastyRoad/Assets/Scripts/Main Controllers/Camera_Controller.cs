using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Camera _mainCamera;
    public Camera mainCamera => _mainCamera;


    // Utility
    /// <summary>
    /// (-1, 0, 1) horizontal(left, right), vertical(top bottom), position x and y(local position)
    /// </summary>
    public Vector2 OuterCamera_Position(float horizontal, float vertical, float positionX, float positionY)
    {
        float horizontalPos = horizontal;
        float verticalPos = vertical;

        if (horizontalPos == 0) horizontalPos = 0.5f;
        else if (horizontalPos == -1f) horizontalPos = 0f;

        if (verticalPos == 0) verticalPos = 0.5f;
        else if (verticalPos == -1f) verticalPos = 0f;

        Vector2 cameraPosition = _mainCamera.ViewportToWorldPoint(new Vector2(horizontalPos, verticalPos));

        return new Vector2(cameraPosition.x + positionX, cameraPosition.y + positionY);
    }
    
    /// <summary>
    /// left is -1, right is 1
    /// </summary>
    public Vector2 OuterCamera_Position(int leftRight)
    {
        // left
        if (leftRight <= 0) return OuterCamera_Position(-1, 0, -2, -3);

        // right
        else return OuterCamera_Position(1, 0, 2, -3);
    }
    
    
    // Camera Position Control
    public void UpdatePosition(Vector2 updatePosition)
    {
        _mainCamera.transform.position = new Vector3(updatePosition.x, updatePosition.y, -10f);
    }

    public void ResetPosition()
    {
        _mainCamera.transform.position = new Vector3(0f, 0f, -10f);
    }
}
