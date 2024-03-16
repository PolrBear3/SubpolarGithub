using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Interaction : MonoBehaviour
{
    private NPC_Controller _controller;

    public delegate void Event();
    public event Event PointerClick_Event;

    [SerializeField] private Transform _dropPoint;
    public Transform dropPoint => _dropPoint;

    private Baggage _baggage;
    public Baggage baggage => _baggage;

    private bool _hasBaggage;
    public bool hasBaggage => _hasBaggage;

    // UnityEngine
    private void Awake()
    {
        _controller = gameObject.GetComponent<NPC_Controller>();
    }

    private void Start()
    {
        PointerClick_Event += Moveto_NextSection;
        PointerClick_Event += ReLine_CurrentSection;
    }

    // Event Trigger
    public void PointerClick()
    {
        PointerClick_Event?.Invoke();
    }

    //
    public void Moveto_NextSection()
    {
        if (_controller.movement.Is_TargetPoint() == false) return;
        if (_controller.currentSection.At_WaitPoint(transform) == false) return; 

        // current
        _controller.currentSection.UnTrack_NPC(_controller);

        // next
        Section_Controller nextSection = _controller.gameController.sections[_controller.currentSection.sectionNum + 1];

        _controller.Update_CurrentSection(nextSection);

        nextSection.Track_NPC(_controller);
        nextSection.Line_NPCs();
    }

    public void ReLine_CurrentSection()
    {
        if (_controller.movement.Is_TargetPoint() == false) return;
        if (_controller.currentSection.At_WaitPoint(transform)) return;

        _controller.currentSection.Line_NPCs();
    }

    //
    public void HasBaggage_Update()
    {
        if (_baggage.dropPoint == _dropPoint)
        {
            _hasBaggage = true;
        }
        else
        {
            _hasBaggage = false;
        }
    }

    public void Set_Baggage(Baggage startBaggage)
    {
        _hasBaggage = true;
        _baggage = startBaggage;
    }
}
