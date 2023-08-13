using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Time_Controller : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Game_Controller _gameController;
    [SerializeField] private List<UI_Bar> _timeBars = new List<UI_Bar>();

    [Header("Value")]
    [SerializeField] private int _maxTime;

    private int _currentTime;
    public int currentTime { get => _currentTime; set => _currentTime = value; }

    private int _currentBarCount;
    public int currentBarCount { get => _currentBarCount; set => _currentBarCount = value; }

    private bool _isNight;
    public bool isNight { get => _isNight; set => isNight = value; }

    //
    private void Start()
    {
        Set_Time(0);
    }

    //
    public void Reset_Time()
    {
        _currentBarCount = 0;

        for (int i = 0; i < _timeBars.Count; i++)
        {
            _timeBars[i].Empty();
        }

        Update_TimePeriod();
    }

    private void Set_Time(int setAmount)
    {
        Reset_Time();

        _currentTime = setAmount;

        int singleBarCount = _maxTime / _timeBars.Count;

        if (_currentTime >= _maxTime) _currentTime = _maxTime;
        else if (_currentTime < singleBarCount) return;

        int barCount = _currentTime / singleBarCount;

        for (int i = 0; i < _timeBars.Count; i++)
        {
            _timeBars[i].Fill();
            barCount--;
            _currentBarCount++;

            if (barCount <= 0) break;
        }

        Update_TimePeriod();
    }

    public void Update_Time(int updateAmount)
    {
        _currentTime += updateAmount;

        if (_currentTime > _maxTime) _currentTime = 0;

        Set_Time(_currentTime);
    }
    public void Update_TimePeriod()
    {
        if (_currentBarCount > 3) _isNight = true;
        else _isNight = false;
    }
}
