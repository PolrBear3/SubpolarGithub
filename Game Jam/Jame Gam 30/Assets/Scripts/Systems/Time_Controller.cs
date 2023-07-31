using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Time_Controller : MonoBehaviour
{
    [SerializeField] private Level_Controller _levelController;

    [Header ("LeanTween Box")]
    [SerializeField] private GameObject _lightBox;
    [SerializeField] private GameObject _darkBox;

    [Header("LeanTween Bar")]
    [SerializeField] private GameObject _lightBar;

    [Header("LeanTween Extra")]
    [SerializeField] private Basic_Gear _gear;
    [SerializeField] private float _transitionTime;
    public float transitionTime { get => _transitionTime; set => _transitionTime = value; }

    [Header("Data Set")]
    [SerializeField] private float _setTime;
    public float setTime { get => _setTime; set => _setTime = value; }

    private float _currentTime;
    public float currentTime { get => _currentTime; set => _currentTime = value; }

    private bool _timeRunning;
    public bool timeRunning { get => _timeRunning; set => _timeRunning = value; }

    //
    private void Update()
    {
        Run_Time();
    }

    // Functions
    public void Start_Game()
    {
        _timeRunning = true;
        _currentTime = _setTime + _transitionTime;

        LeanTween.moveLocalX(_gear.gameObject, 1f, _setTime).setDelay(_transitionTime);

        LeanTween.alpha(_lightBox, 0.1f, _setTime).setDelay(_transitionTime);
        LeanTween.alpha(_lightBar, 1f, _setTime).setDelay(_transitionTime);
    }
    public void End_Game()
    {
        _timeRunning = false;
        _currentTime = _setTime;

        _gear.goldGear.DarkGear_Update();
        _gear.spinningRight = !_gear.spinningRight;

        LeanTween.cancel(_gear.gameObject);
        LeanTween.moveLocalX(_gear.gameObject, -1f, _transitionTime);

        LeanTween.cancel(_lightBox);
        LeanTween.alpha(_lightBox, 0f, _transitionTime);

        LeanTween.cancel(_lightBar);
        LeanTween.alpha(_lightBar, 0f, _transitionTime);

        LeanTween.alpha(_darkBox, 1f, _transitionTime).setDelay(_transitionTime);

        _levelController.Next_Level(4f);
    }

    private void Run_Time()
    {
        if (!_timeRunning) return;
        if (_currentTime <= 0) return;
        _currentTime -= Time.deltaTime;
    }
}
