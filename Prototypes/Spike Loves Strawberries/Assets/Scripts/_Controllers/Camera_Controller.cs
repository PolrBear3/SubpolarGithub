using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public static Camera_Controller instance;
    
    
    [SerializeField] private Camera _mainCamera;

    [Space(20)] 
    [SerializeField] private Vector2 _updateDistanceValue;
    
    [Space(20)] 
    [SerializeField] [Range(0, 10)] private float _tweenSpeed;
    public float tweenSpeed => _tweenSpeed;
    
    [SerializeField] private LeanTweenType _tweenType;


    private Vector2 _targetPosition;
    public Vector2 targetPosition => _targetPosition;
    
    
    // MonoBehaviour
    private void Awake()
    {
        instance = this;
    }


    // Control
    public void Set_CameraPosition(Vector2 setPosition)
    {
        LeanTween.move(_mainCamera.gameObject, setPosition, _tweenSpeed).setEase(_tweenType);
    }
    
    
    private Vector2 Update_Direction(Vector2 direction)
    {
        float xDirection = 0f;
        float yDirection = 0f;

        if (direction.x > 0) xDirection = _updateDistanceValue.x;
        else if (direction.x < 0) xDirection = -_updateDistanceValue.x;

        if (direction.y > 0) yDirection = _updateDistanceValue.y;
        else if (direction.y < 0) yDirection = -_updateDistanceValue.y;

        return new Vector2(xDirection, yDirection);
    }

    public void Update_CameraPosition(Vector2 direction)
    {
        Vector2 updateDirection = Update_Direction(direction);
        Vector2 cameraPosition = _mainCamera.transform.position;
        
        Vector2 updatePosition = new(cameraPosition.x + updateDirection.x, cameraPosition.y + updateDirection.y);
        _targetPosition = updatePosition;
        
        LeanTween.move(_mainCamera.gameObject, updatePosition, _tweenSpeed).setEase(_tweenType);
    }
}
