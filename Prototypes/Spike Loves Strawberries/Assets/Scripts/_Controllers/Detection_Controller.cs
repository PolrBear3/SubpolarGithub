using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection_Controller : MonoBehaviour
{
    private Spike _detectedPlayer;
    public Spike detectedPlayer => _detectedPlayer;
    
    private bool _playerDetected;
    public bool playerDetected => _playerDetected;
    
    public Action OnPlayerDetect;
    public Action OnPlayerExit;
    
    
    private List<GameObject> _detectedObjects = new();
    public List<GameObject> detectedObjects => _detectedObjects;

    public Action OnObjectDetect;
    public Action OnObjectExit;


    // onTrigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        Track_DetectedObjects(other.gameObject, true);
        
        if (other.TryGetComponent(out Spike player) == false) return;
        
        _detectedPlayer = player;
        
        _playerDetected = true;
        OnPlayerDetect?.Invoke();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        Track_DetectedObjects(other.gameObject, false);
        
        if (other.TryGetComponent(out Spike player) == false) return;
        
        _playerDetected = false;
        OnPlayerExit?.Invoke();
        
        _detectedPlayer = null;
    }
    
    
    // Other
    private void Track_DetectedObjects(GameObject trackObject, bool track)
    {
        if (trackObject.TryGetComponent(out Detection_Controller detection) == false) return;

        if (track == false)
        {
            _detectedObjects.Remove(trackObject);
            OnObjectExit?.Invoke();
            
            return;
        }
        _detectedObjects.Add(trackObject);
        OnObjectDetect?.Invoke();
    }
}
