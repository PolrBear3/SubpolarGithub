using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location_Controller : MonoBehaviour
{
    private Game_Controller _gameController;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }
    private void Start()
    {
        _gameController.Connect_Location(this);
    }
}
