using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baggage_CheckPoint : MonoBehaviour
{
    private Game_Controller _gameController;

    [SerializeField] private List<Baggage> _currentBaggages = new();
    public List<Baggage> currentBaggages => _currentBaggages;

    public delegate void Event();
    public event Event SetBaggage_Event;

    private int _checkPointNum;
    public int checkPointNum => _checkPointNum;



    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }



    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage bag)) return;
        if (bag.dragging) return;
        if (bag.ownerNPC.interaction.hasBaggage) return;

        _gameController.checkPoints[bag.checkNum].Remove_Baggage(bag);

        Set_Baggage(bag);
        bag.Movement_Toggle(false);
    }



    // for game controller start setting
    public void Set_CheckPointNum(int num)
    {
        _checkPointNum = num;
    }



    //
    public bool Has_Baggage(Baggage bag)
    {
        for (int i = 0; i < _currentBaggages.Count; i++)
        {
            if (bag != _currentBaggages[i]) continue;
            return true;
        }
        return false;
    }

    public void Remove_Baggage(Baggage bag)
    {
        for (int i = 0; i < _currentBaggages.Count; i++)
        {
            if (bag != _currentBaggages[i]) continue;
            _currentBaggages.RemoveAt(i);
            break;
        }
    }

    public void Set_Baggage(Baggage bag)
    {
        if (Has_Baggage(bag) == false)
        {
            _currentBaggages.Add(bag);
        }

        bag.transform.parent = transform;
        bag.transform.localPosition = Vector2.zero;

        SetBaggage_Event?.Invoke();
    }
}
