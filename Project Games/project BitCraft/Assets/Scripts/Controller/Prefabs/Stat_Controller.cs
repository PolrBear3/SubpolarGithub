using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat_Controller : MonoBehaviour
{
    [Header("Max Stats")]
    [SerializeField] private int _maxLifeCount;
    public int maxLifeCount { get => _maxLifeCount; set => _maxLifeCount = value; }
    [SerializeField] private int _maxFatigue;
    public int maxFatigue { get => _maxFatigue; set => _maxFatigue = value; }
    [SerializeField] private int _maxHunger;
    public int maxHunger { get => _maxHunger; set => _maxHunger = value; }
    [SerializeField] private int _maxLungCapacity;
    public int maxLungCapacity { get => _maxLungCapacity; set => _maxLungCapacity = value; }

    [Header("Current Stats")]
    [SerializeField] private int _currentLifeCount;
    public int currentLifeCount
    {
        get => _currentLifeCount;
        set
        {
            _currentLifeCount = value;

            if (_currentLifeCount >= _maxLifeCount)
            {
                _currentLifeCount = _maxLifeCount;
            }
            else if (_currentLifeCount <= 0)
            {
                _currentLifeCount = 0;
            }
        }
    }
    [SerializeField] private int _currentFatigue;
    public int currentFatigue
    {
        get => _currentFatigue;
        set
        {
            _currentFatigue = value;

            if (_currentFatigue >= _maxFatigue)
            {
                _currentFatigue = _maxFatigue;
            }
            else if (_currentFatigue <= 0)
            {
                _currentFatigue = 0;
            }
        }
    }

    //
    private void Start()
    {
        Reset_Current_Stats();
    }

    // Set
    private void Reset_Current_Stats()
    {
        _currentLifeCount = _maxLifeCount;
        _currentFatigue = _maxFatigue;
    }

    // Functions
    public void Update_Current_Life(int value)
    {
        currentLifeCount += value;
    }
    public void Update_Current_Fatigue(int value)
    {
        currentFatigue += value;
    }
}
