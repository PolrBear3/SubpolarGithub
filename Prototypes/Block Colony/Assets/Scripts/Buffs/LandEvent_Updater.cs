using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandEvent_Updater : MonoBehaviour, ILandInteractable, IInteractCheck
{
    private MainController _main;

    [SerializeField] private EventScrObj updateEventScrObj;

    [SerializeField] private LandType[] _uninteractableLands;

    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }


    // IInteractCheck
    public bool InteractAvailable()
    {
        Land hoverLand = _main.cursor.hoveringObject.GetComponent<Land_SnapPoint>().currentData.currentLand;

        for (int i = 0; i < _uninteractableLands.Length; i++)
        {
            if (hoverLand.currentData.type == _uninteractableLands[i]) return false; 
        }
        return true;
    }


    // ILandInteractable
    public void Interact()
    {
        Land hoverLand = _main.cursor.hoveringObject.GetComponent<Land_SnapPoint>().currentData.currentLand;
        hoverLand.currentData.Update_Event(updateEventScrObj);
    }
}
