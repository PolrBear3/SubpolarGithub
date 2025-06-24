using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private Detection_Controller _detection;

    [Space(20)] 
    [SerializeField][Range(0, 10)] private float _damageTime;
    
    
    private Coroutine _coroutine;
    
    
    // MonoBehaviour
    private void Start()
    {

    }

    private void OnDestroy()
    {

    }
    
    
    // Damage
}
