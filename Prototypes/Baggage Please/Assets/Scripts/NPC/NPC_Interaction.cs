using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Interaction : MonoBehaviour
{
    private NPC_Controller _controller;

    public delegate void Event();
    public event Event PointerClick_Event;

    private SpriteRenderer _sr;

    private Baggage _baggage;
    public Baggage baggage => _baggage;

    private bool _hasBaggage;
    public bool hasBaggage => _hasBaggage;

    private bool _arrested;
    public bool arrested => _arrested;

    [SerializeField] private Transform _baggagePoint;
    public Transform baggagePoint => _baggagePoint;



    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _controller = gameObject.GetComponent<NPC_Controller>();
    }

    private void Start()
    {
        Drop_Baggage();

        PointerClick_Event += Moveto_NextSection;
        PointerClick_Event += ReLine_CurrentSection;
    }



    // Event Trigger
    public void PointerClick()
    {
        PointerClick_Event?.Invoke();
    }



    // Section and Line Control
    public void Moveto_NextSection()
    {
        if (_controller.movement.Is_TargetPoint() == false) return;
        if (_controller.currentSection.At_WaitPoint(transform) == false) return; 

        // current
        _controller.currentSection.UnTrack_NPC(_controller);

        // go to end point
        if (_controller.currentSection.sectionNum + 1 >= _controller.gameController.sections.Count)
        {
            _controller.gameController.UnTrack_NPC(_controller);
            _controller.movement.Set_TargetPoint(_controller.gameController.endPoint.position);
            return;
        }

        // next
        Section_Controller nextSection = _controller.gameController.sections[_controller.currentSection.sectionNum + 1];

        _controller.Update_CurrentSection(nextSection);

        nextSection.Track_NPC(_controller);
        nextSection.Line_NPCs();
    }

    public void Moveto_Section(int sectionNum)
    {
        // current
        _controller.currentSection.UnTrack_NPC(_controller);

        // go to end point
        if (_controller.currentSection.sectionNum + 1 >= _controller.gameController.sections.Count)
        {
            _controller.gameController.UnTrack_NPC(_controller);
            _controller.movement.Set_TargetPoint(_controller.gameController.endPoint.position);
            return;
        }

        // next
        Section_Controller nextSection = _controller.gameController.sections[sectionNum];

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



    // Baggage Control
    public void Set_Baggage(Baggage startBaggage)
    {
        _hasBaggage = true;
        _baggage = startBaggage;

        _baggage.transform.parent = transform;
        _baggage.transform.localPosition = _baggagePoint.localPosition;
    }

    public void Drop_Baggage()
    {
        StartCoroutine(Drop_Baggage_Coroutine());
    }
    private IEnumerator Drop_Baggage_Coroutine()
    {
        while (_hasBaggage)
        {
            yield return new WaitForSeconds(1f);

            if (_controller.currentSection.At_WaitPoint(transform) == false) continue;

            _hasBaggage = false;
            _controller.gameController.checkPoints[_baggage.checkNum].Set_Baggage(_baggage);

            break;
        }
    }

    public void Collect_Baggage()
    {
        Set_Baggage(_baggage);
    }



    //
    public void LeaveScore_Update()
    {
        int bagHeatLevel = _baggage.data.heatLevel;
        int scoreCount = 0;

        if (_arrested)
        {
            if (bagHeatLevel >= 3) scoreCount += 6;
            else scoreCount -= 3 - bagHeatLevel;
        }
        else
        {
            if (bagHeatLevel <= 0) scoreCount--;
            else scoreCount += bagHeatLevel;
        }

        Data_Controller.score += scoreCount;
        _controller.gameController.data.ScoreText_Update();
    }

    public void Arrest_Toggle(bool toggleOn)
    {
        _arrested = toggleOn;
    }
}
