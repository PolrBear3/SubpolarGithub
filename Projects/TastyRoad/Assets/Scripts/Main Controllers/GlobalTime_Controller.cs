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
    [SerializeField][Range(0, 100)] private float _tikTime;
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
        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        while (true)
        {
            yield return new WaitForSeconds(_tikTime);

            // increase check
            if (_currentTime >= 12)
            {
                _isIncrease = false;

                // after noon day tik dialog
                DialogData dayDialog = new DialogData(dialog.datas[2].icon, "Current time phase is after noon");
                dialog.Update_Dialog(dayDialog);
            }
            else if (_currentTime <= 0)
            {
                _isIncrease = true;

                // before noon day tik dialog
                DialogData nightDialog = new DialogData(dialog.datas[0].icon, "Current time phase is before noon");
                dialog.Update_Dialog(nightDialog);
            }

            // value update
            if (_isIncrease)
            {
                _currentTime++;

                // before noon tik dialog
                DialogData dayDialog = new DialogData(dialog.datas[1].icon, "Current time is " + _currentTime + " before noon");
                dialog.Update_Dialog(dayDialog);
            }
            else
            {
                _currentTime--;

                // after noon tik dialog
                DialogData nightDialog = new DialogData(dialog.datas[3].icon, "Current time is " + (12 - _currentTime) + " after noon");
                dialog.Update_Dialog(nightDialog);
            }

            // Time Tik Event
            TimeTik_Update?.Invoke();

            // Day Tik Event
            if (_currentTime == 0) DayTik_Update?.Invoke();
        }
    }
}