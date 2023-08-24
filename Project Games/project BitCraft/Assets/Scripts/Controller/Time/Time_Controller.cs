using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Time_Controller : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Game_Controller _gameController;

    [Header("Value")]
    [SerializeField] private int _maxTime;
    [SerializeField] private List<UI_Bar> _timeBars = new List<UI_Bar>();
    [SerializeField]private int _currentTime;
    public int currentTime { get => _currentTime; set => _currentTime = value; }

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
        _currentTime = 0;

        for (int i = 0; i < _timeBars.Count; i++)
        {
            _timeBars[i].Empty();
        }

        Update_TimePeriod();
    }

    private void Set_Time(int setAmount)
    {
        _currentTime = setAmount;

        int singleBarCount = _maxTime / _timeBars.Count;

        if (_currentTime >= _maxTime) _currentTime = _maxTime;
        else if (_currentTime < singleBarCount) return;

        int barCount = _currentTime / singleBarCount;

        for (int i = 0; i < _timeBars.Count; i++)
        {
            if (barCount > 0)
            {
                _timeBars[i].Fill();
                barCount--;
                continue;
            }

            _timeBars[i].Empty();
        }

        Update_TimePeriod();
    }
    public void Update_Time(int updateAmount)
    {
        _currentTime += updateAmount;

        if (_currentTime > _maxTime) Reset_Time();

        Set_Time(_currentTime);
        _gameController.tilemapController.renderSystem.Update_Player_ShadowMode();
    }

    public void Update_TimePeriod()
    {
        int nightTime = _maxTime - _maxTime / 3;

        if (_currentTime > nightTime) _isNight = true;
        else _isNight = false;
    }
}
