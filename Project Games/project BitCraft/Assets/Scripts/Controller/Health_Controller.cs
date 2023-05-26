using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Controller : MonoBehaviour
{
    [SerializeField] private int _maxLifeCount;
    public int maxLifeCount { get => _maxLifeCount; set => _maxLifeCount = value; }

    private int _currentLifeCount;
    public int currentLifeCount
    {
        get => _currentLifeCount;
        set
        {
            _currentLifeCount = value;
            
            if (_currentLifeCount <= _maxLifeCount) return;
            _currentLifeCount = maxLifeCount;
        }
    }

    private void Start()
    {
        Set_LifeCount();
    }

    public void Set_LifeCount()
    {
        _currentLifeCount = _maxLifeCount;
    }

    public void Add_Current_LifeCount(int amount)
    {
        currentLifeCount += amount;
    }
    public void Subtract_Current_LifeCount(int amount)
    {
        currentLifeCount -= amount;
    }
}