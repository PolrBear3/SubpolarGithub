using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat_Controller : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Game_Controller _gameController;

    [Header("Value")]
    [SerializeField] private List<UI_Bar> _fatigueBars = new List<UI_Bar>();
    [SerializeField] private int _maxFatigue;
    private int _currentFatigue;
    public int currentFatigue { get => _currentFatigue; set => _currentFatigue = value; }

    //
    private void Start()
    {
        Set_Fatigue(18);
    }

    // Stat Control
    public void Set_Fatigue(int amount)
    {
        _currentFatigue = amount;

        int singleBarAmount = _maxFatigue / _fatigueBars.Count;

        if (_currentFatigue >= _maxFatigue) _currentFatigue = _maxFatigue;
        else if (_currentFatigue < singleBarAmount) return;

        int barCount = _currentFatigue / singleBarAmount;

        for (int i = 0; i < _fatigueBars.Count; i++)
        {
            if (barCount > 0)
            {
                _fatigueBars[i].Fill();
                barCount--;
                continue;
            }

            _fatigueBars[i].Empty();
        }
    }
    public void Update_Fatigue(int amount)
    {
        _currentFatigue += amount;
        Set_Fatigue(_currentFatigue);
    }
}
