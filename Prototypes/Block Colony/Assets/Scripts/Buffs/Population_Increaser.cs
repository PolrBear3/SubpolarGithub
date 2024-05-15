using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population_Increaser : MonoBehaviour, ILandInteractable
{
    private MainController _main;

    [SerializeField] private EventScrObj _updateEventScrObj;
    public EventScrObj updateEventScrObj => _updateEventScrObj;

    [SerializeField] private int _updateIncrease;


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }


    // ILandInteractable
    public void Interact()
    {
        Land hoverLand = _main.cursor.hoveringObject.GetComponent<Land_SnapPoint>().currentData.currentLand;
        hoverLand.currentData.Update_Population(_updateIncrease);
    }
}
