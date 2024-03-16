using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baggage_DropPoint : MonoBehaviour
{
    private List<Baggage> _detectedBaggages = new();
    public List<Baggage> detectedBaggeges => _detectedBaggages;

    /*
    public delegate void Event();
    public event Event Bag_DetectEvent;
    */

    [SerializeField] private Transform _baggagePosition;
    public Transform baggagePosition => _baggagePosition;

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage bag)) return;
        if (bag.droppable == false) return;

        _detectedBaggages.Add(bag);

        if (bag.dragging) return;

        bag.Set_DropPoint(_baggagePosition);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage bag)) return;
        _detectedBaggages.Remove(bag);
    }
}
