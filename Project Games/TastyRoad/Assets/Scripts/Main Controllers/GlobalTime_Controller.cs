using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalTime_Controller : MonoBehaviour
{
    [SerializeField] private Light2D _globalLight;

    /// <summary>
    /// Current time range is 30 ~ 100
    /// </summary>
    public int currentTime => _currentTime;
    private int _currentTime;

    [Header("")]
    [SerializeField] private float _tikTime;

    public delegate void OnEvent();
    public event OnEvent TimeTik_Update;


    // UnityEngine
    private void Start()
    {
        GlobalTime_Update();
    }



    //
    private void GlobalTime_Update()
    {
        StartCoroutine(GlobalTime_Update_Coroutine());
    }
    private IEnumerator GlobalTime_Update_Coroutine()
    {
        bool isIncrease = false;

        while (true)
        {
            if (isIncrease == false)
            {
                _globalLight.intensity -= 0.01f;

                if (_globalLight.intensity <= 0.3f) isIncrease = true;
            }
            else
            {
                _globalLight.intensity += 0.01f;

                if (_globalLight.intensity >= 1f) isIncrease = false;
            }

            _currentTime = (int)Mathf.Floor(_globalLight.intensity * 100);

            TimeTik_Update?.Invoke();

            yield return new WaitForSeconds(_tikTime);
        }
    }
}