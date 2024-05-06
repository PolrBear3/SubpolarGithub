using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandEvent_Updater : MonoBehaviour, ILandInteractable
{
    private MainController _main;

    [SerializeField] private EventScrObj updateEventScrObj;

    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }


    // ILandInteractable
    public void Interact()
    {
        Land hoverLand = _main.cursor.hoveringObject.GetComponent<Land_SnapPoint>().currentData.currentLand;
        hoverLand.currentData.Update_Event(updateEventScrObj);

        // update landSprite on hoverLand //
    }
}
