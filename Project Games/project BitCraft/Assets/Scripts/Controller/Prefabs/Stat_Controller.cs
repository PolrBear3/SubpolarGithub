using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat_Controller : MonoBehaviour
{
    [Header("Max Stats")]
    [SerializeField] private int _maxLifeCount;
    [SerializeField] private int _maxFatigue;
    [SerializeField] private int _maxHunger;
    [SerializeField] private int _maxLungCapacity;

    [Header("Current Stats")]
    [SerializeField] private int _currentLifeCount;
    public int currentLifeCount
    {
        get => _currentLifeCount;
        set
        {
            _currentLifeCount = value;

            if (_currentLifeCount <= _maxLifeCount) return;
            _currentLifeCount = _maxLifeCount;
        }
    }
    [SerializeField] private int _currentFatigue;
    public int currentFatigue
    {
        get => _currentFatigue;
        set
        {
            _currentFatigue = value;

            if (_currentFatigue <= _maxFatigue) return;
            _currentLifeCount = _maxFatigue;
        }
    }
    [SerializeField] private int _currentHunger;
    public int currentHunger
    {
        get => _currentHunger;
        set
        {
            _currentHunger = value;

            if (_currentHunger <= _maxHunger) return;
            _currentHunger = _maxHunger;
        }
    }
    [SerializeField] private int _currentLungCapacity;
    public int currentLungCapacity
    {
        get => _currentLungCapacity;
        set
        {
            _currentLifeCount = value;

            if (_currentLungCapacity <= _maxLungCapacity) return;
            _currentLungCapacity = _maxLungCapacity;
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
        _currentHunger = _maxHunger;
        _currentLungCapacity = _maxLungCapacity;
    }

    // Functions
    public void Update_Current_LifeCount(int amount)
    {
        _currentLifeCount += amount;
    }
}
