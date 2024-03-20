using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite_ToggleBox : MonoBehaviour
{
    private SpriteRenderer _sr;

    [SerializeField] private bool _toggleOn;

    public delegate void Event();
    public event Event TriggerEnter_Event;

    private Baggage _detectedBaggage;
    public Baggage detectedBaggage => _detectedBaggage;



    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _sr.color = Color.clear;
    }



    // On Triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage bag)) return;
        if (bag.dragging) return;
        if (bag.ownerNPC.interaction.hasBaggage) return;

        _detectedBaggage = bag;

        bag.Sprite_Toggle(_toggleOn);
        TriggerEnter_Event?.Invoke();
    }
}
