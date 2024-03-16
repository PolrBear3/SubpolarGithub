using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baggage_HidePoint : MonoBehaviour
{
    private SpriteRenderer _sr;

    private List<Baggage> _detectedBaggages = new();
    public List<Baggage> detectedBaggeges => _detectedBaggages;

    /*
    public delegate void Event();
    public event Event Bag_DetectEvent;
    */

    [SerializeField] private Transform _baggagePosition;
    public Transform baggagePosition => _baggagePosition;

    [SerializeField] private bool _isShowPoint;

    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _sr.color = Color.clear;
    }

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage bag)) return;
        if (bag.droppable == false) return;

        _detectedBaggages.Add(bag);

        bag.Set_DropPoint(_baggagePosition);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage bag)) return;
        _detectedBaggages.Remove(bag);
    }
}
