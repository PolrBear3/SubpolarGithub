using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public enum TimePhase { Day, Night }

public class GlobalTime_Controller : MonoBehaviour, ISaveLoadable
{
    public static GlobalTime_Controller instance;


    [Space(20)]
    [SerializeField][Range(0, 100)] private float _tikDelayTime;
    [SerializeField][Range(0, 24)] private int _startingNightTime;

    [Space(20)]
    [SerializeField] private Color _nightColor;
    [SerializeField][Range(0, 10)] private float _transitionDuration;


    private GlobalTime_Data _data;
    public GlobalTime_Data data => _data;


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
        Color updateColor = _data.timePhase == TimePhase.Day ? Color.white : _nightColor;
        
        lightController.Set_DefaultColor(updateColor);
        lightController.Update_CurrentColor(updateColor, 0);
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("GlobalTime_Controller/GlobalTime_Data", _data);
    }

    public void Load_Data()
    {
        _data = ES3.Load("GlobalTime_Controller/GlobalTime_Data", new GlobalTime_Data(TimePhase.Day));
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

            int setTime = (_data.timeCount + 1) % 24;
            _data.Set_TimeCount(setTime);
            
            Toggle_TimePhase();
            OnTimeTik?.Invoke();

            Debug.Log(_data.timePhase + " " + _data.timeCount);
        }
    }

    private void Toggle_TimePhase()
    {
        TimePhase recentPhase = _data.timePhase;
        _data.Set_TimePhase((TimePhase)Convert.ToInt32(_data.timeCount >= _startingNightTime));

        if (_data.timePhase == recentPhase) return;

        GlobalLight_Controller lightController = Main_Controller.instance.globalLightController;
        Color updateColor = _data.timePhase == TimePhase.Day ? Color.white : _nightColor;
        
        lightController.Set_DefaultColor(updateColor);
        lightController.Update_CurrentColor(updateColor, _transitionDuration);

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_data.timePhase == TimePhase.Night)
        {
            OnNightTime?.Invoke();
            dialog.Update_Dialog(1);

            return;
        }

        OnDayTime?.Invoke();
        dialog.Update_Dialog(0);
    }
}