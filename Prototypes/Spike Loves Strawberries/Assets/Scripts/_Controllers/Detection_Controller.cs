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


    // onTrigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Spike player) == false) return;
        
        _detectedPlayer = player;
        
        _playerDetected = true;
        OnPlayerDetect?.Invoke();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Spike player) == false) return;
        
        _playerDetected = false;
        OnPlayerExit?.Invoke();
        
        _detectedPlayer = null;
    }
}
