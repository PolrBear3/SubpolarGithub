using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public enum TimePhase { Day, Night }

public class globaltime : MonoBehaviour, ISaveLoadable
{
    public static globaltime instance;


    [Header("")]
    [SerializeField][Range(0, 100)] private float _tikDelayTime;
    [SerializeField][Range(0, 24)] private int _startingNightTime;

    [Header("")]
    [SerializeField] private Color _nightColor;
    [SerializeField][Range(0, 10)] private float _transitionDuration;


    private int _currentTime;
    public int currentTime => _currentTime;

    private TimePhase _currentTimePhase;
    public TimePhase currentTimePhase => _currentTimePhase;


    public Action OnTimeTik;

    public Action OnDayTime;
    public Action OnNightTime;


    private Coroutine _lightCoroutine;



    // UnityEngine
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CycleTime();
        
        GlobalLight_Controller lightController = Main_Controller.instance.globalLightController;
        Color updateColor = _currentTimePhase == TimePhase.Day ? Color.white : _nightColor;
        
        lightController.Set_DefaultColor(updateColor);
        lightController.Update_CurrentColor(updateColor, 0);
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("GlobalTime_Controller/_currentTime", _currentTime);
        ES3.Save("GlobalTime_Controller/_currentTimePhase", _currentTimePhase);
    }

    public void Load_Data()
    {
        _currentTime = ES3.Load("GlobalTime_Controller/_currentTime", _currentTime);
        _currentTimePhase = ES3.Load("GlobalTime_Controller/_currentTimePhase", _currentTimePhase);
    }


    //
    private void CycleTime()
    {
        StartCoroutine(CycleTime_Corountine());
    }
    private IEnumerator CycleTime_Corountine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_tikDelayTime);

            _currentTime = (_currentTime + 1) % 24;
            OnTimeTik?.Invoke();

            Debug.Log(_currentTime);

            Toggle_TimePhase();
        }
    }

    private void Toggle_TimePhase()
    {
        TimePhase recentPhase = _currentTimePhase;
        _currentTimePhase = (TimePhase)Convert.ToInt32(_currentTime >= _startingNightTime);

        if (_currentTimePhase == recentPhase) return;

        GlobalLight_Controller lightController = Main_Controller.instance.globalLightController;
        Color updateColor = _currentTimePhase == TimePhase.Day ? Color.white : _nightColor;
        
        lightController.Set_DefaultColor(updateColor);
        lightController.Update_CurrentColor(updateColor, _transitionDuration);

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_currentTimePhase == TimePhase.Night)
        {
            OnNightTime?.Invoke();
            dialog.Update_Dialog(1);

            return;
        }

        OnDayTime?.Invoke();
        dialog.Update_Dialog(0);
    }
}