using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Camera _mainCamera;
    public Camera mainCamera => _mainCamera;


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
