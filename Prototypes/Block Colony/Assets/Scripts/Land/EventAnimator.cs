using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAnimator : MonoBehaviour
{
    private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;

    private Animator _anim;
    public Animator anim => _anim;

    private EventScrObj _currentEvent;
    public EventScrObj currentEvent => _currentEvent;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _anim = gameObject.GetComponent<Animator>();
    }


    // Functions
    public void Play_EventAnimation(EventScrObj playEvent)
    {
        _currentEvent = playEvent;

        if (playEvent.eventAnimation == null) return; 

        _anim.runtimeAnimatorController = playEvent.eventAnimation;
        _anim.Play("EventAnimator_play");
    }
}
