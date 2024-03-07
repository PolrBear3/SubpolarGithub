using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalTime_Controller : MonoBehaviour, ISaveLoadable
{
    [SerializeField] private Light2D _globalLight;

    /// <summary>
    /// Current time range is 30 ~ 100
    /// </summary>
    public int currentTime => _currentTime;
    private int _currentTime;

    [Header("")]
    [SerializeField] private float _tikTime;
    private bool _isIncrease;

    public delegate void OnEvent();
    public event OnEvent TimeTik_Update;



    // UnityEngine
    private void Start()
    {
        GlobalTime_Update();
    }



    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("_isIncrease", _isIncrease);
        ES3.Save("_currentTime", _currentTime);
        ES3.Save("_globalLightIntensity", _globalLight.intensity);
    }

    public void Load_Data()
    {
        _isIncrease = ES3.Load("_isIncrease", _isIncrease);
        _currentTime = ES3.Load("_currentTime", _currentTime);
        _globalLight.intensity = ES3.Load("_globalLightIntensity", _globalLight.intensity);
    }



    //
    private void GlobalTime_Update()
    {
        StartCoroutine(GlobalTime_Update_Coroutine());

        _currentTime = (int)Mathf.Floor(_globalLight.intensity * 100);
    }
    private IEnumerator GlobalTime_Update_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_tikTime);

            if (_isIncrease == false)
            {
                if (_currentTime <= 30)
                {
                    _isIncrease = true;
                    _globalLight.intensity += 0.01f;
                }
                else _globalLight.intensity -= 0.01f;
            }
            else
            {
                if (_currentTime >= 100)
                {
                    _isIncrease = false;
                    _globalLight.intensity -= 0.01f;
                }
                else _globalLight.intensity += 0.01f;
            }

            _currentTime = (int)Mathf.Floor(_globalLight.intensity * 100);

            TimeTik_Update?.Invoke();
        }
    }
}