using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineDetector : MonoBehaviour
{
    private Game_Controller _gameController;
    public Game_Controller gameController => _gameController;

    [SerializeField] private Baggage_CheckPoint _movePoint;
    [SerializeField] private Sprite_ToggleBox _heatDetectPoint;



    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    private void Start()
    {
        _movePoint.SetBaggage_Event += Move_Baggage;
        _heatDetectPoint.TriggerEnter_Event += BaggageCheck_HeatLevel;
    }



    //
    private void Move_Baggage()
    {
        for (int i = 0; i < _movePoint.currentBaggages.Count; i++)
        {
            _movePoint.currentBaggages[i].Movement_Toggle(true);
        }
    }

    private void BaggageCheck_HeatLevel()
    {
        _heatDetectPoint.detectedBaggage.Check_HeatLevel();
    }
}