using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Time_Controller : MonoBehaviour
{
    [Header("Inspector Set")]
    [SerializeField] private Game_Controller _gameController;

    [SerializeField] private List<Time_Slot> _timeSlot = new List<Time_Slot>();
    public List<Time_Slot> timeSlot { get => _timeSlot; set => _timeSlot = value; }

    private int _currentTimeCount;
    public int currentTimeCount { get => _currentTimeCount; set => _currentTimeCount = value; }
}
