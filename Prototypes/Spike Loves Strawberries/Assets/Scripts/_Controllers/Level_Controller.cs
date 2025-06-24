using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Controller : MonoBehaviour
{
    public static Level_Controller instance;
    
    
    [Space(20)]
    [SerializeField] private Spike _player;
    public Spike player => _player;
    
    
    // MonoBehaviour
    private void Awake()
    {
        instance = this;
    }
}
