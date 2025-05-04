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
    [SerializeField][Range(0, 1)] private float _transitionSpeed;


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

        bool isDayTime = currentTimePhase == TimePhase.Day;
        Main_Controller.instance.globalLight.color = isDayTime ? Color.white : _nightColor;
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

        Update_LightAtmosphere();

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


    // Global Light
    private void Update_LightAtmosphere()
    {
        if (_lightCoroutine != null)
        {
            StopCoroutine(_lightCoroutine);
            _lightCoroutine = null;
        }

        bool isDayTime = currentTimePhase == TimePhase.Day;
        Color targetColor = isDayTime ? Color.white : _nightColor;

        _lightCoroutine = StartCoroutine(Update_LightAtmosphere_Coroutine(targetColor));
    }
    private IEnumerator Update_LightAtmosphere_Coroutine(Color targetColor)
    {
        Main_Controller main = Main_Controller.instance;

        Color startColor = main.globalLight.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * _transitionSpeed;
            main.globalLight.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        main.globalLight.color = targetColor;

        _lightCoroutine = null;
        yield break;
    }
}