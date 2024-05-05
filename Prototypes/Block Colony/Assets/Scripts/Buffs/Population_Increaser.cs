using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population_Increaser : MonoBehaviour, ILandInteractable
{
    private MainController _main;

    [SerializeField] private int _increaseAmount;


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }


    // ILandInteractable
    public void Interact()
    {
        Land hoverLand = _main.cursor.hoveringObject.GetComponent<Land_SnapPoint>().currentData.currentLand;

        Debug.Log(hoverLand.currentData.population);

        hoverLand.currentData.Update_Population(_increaseAmount);

        Debug.Log(hoverLand.currentData.population);
    }
}
