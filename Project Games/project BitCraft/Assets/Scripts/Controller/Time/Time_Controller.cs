using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Time_Controller : MonoBehaviour
{
    [Header("Inspector Set")]
    [SerializeField] private Game_Controller _gameController;

    private int _currentTime;
    public int currentTime { get => _currentTime; set => _currentTime = value; }
}
