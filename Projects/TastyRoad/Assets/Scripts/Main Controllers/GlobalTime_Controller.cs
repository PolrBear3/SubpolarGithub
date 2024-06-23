using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalTime_Controller : MonoBehaviour, ISaveLoadable
{
    /// <summary>
    /// Current time range is 0 ~ 12
    /// </summary>
    private int _currentTime;
    public int currentTime => _currentTime;

    [Header("")]
    [SerializeField] private float _tikTime;
    private bool _isIncrease;

    public delegate void OnEvent();
    public static event OnEvent TimeTik_Update;
    public static event OnEvent DayTik_Update;


    // UnityEngine
    private void Start()
    {
        GlobalTime_Update();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("_currentTime", _currentTime);
    }

    public void Load_Data()
    {
        _currentTime = ES3.Load("_currentTime", _currentTime);
    }


    //
    private void GlobalTime_Update()
    {
        StartCoroutine(GlobalTime_Update_Coroutine());
    }
    private IEnumerator GlobalTime_Update_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_tikTime);

            //
            if (_currentTime >= 12) _isIncrease = false;
            else if (_currentTime <= 0) _isIncrease = true;

            if (_isIncrease) _currentTime++;
            else _currentTime--;

            // Time Tik
            TimeTik_Update?.Invoke();

            // Day Tik
            if (_currentTime == 0) DayTik_Update?.Invoke();
        }
    }
}