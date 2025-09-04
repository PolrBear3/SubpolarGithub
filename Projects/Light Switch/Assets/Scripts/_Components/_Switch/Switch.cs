using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private Clickable_EventSystem _eventSystem;
    
    
    // MonoBehaviour
    private void Start()
    {
        _eventSystem.OnClick += Toggle;
    }

    private void OnDestroy()
    {
        _eventSystem.OnClick -= Toggle;
    }


    // Main
    private void Toggle()
    {
        Debug.Log("This is Switch");
    }
}
