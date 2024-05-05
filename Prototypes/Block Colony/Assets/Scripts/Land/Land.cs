using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILandInteractable
{
    void Interact();
}

public class Land : MonoBehaviour, ISnapPointInteractable
{
    private MainController _main;

    [SerializeField] private LandData _setData;
    public LandData setData => _setData;

    private LandData _currentData;
    public LandData currentData => _currentData;


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }

    private void Start()
    {
        Set_CurrentData(_setData);
    }


    // ISnapPointInteractable and EventTrigger
    public void Interact()
    {
        Place_CurrentLand();
    }

    public void OnPointerClick()
    {

    }


    // Set Functions
    public void Set_CurrentData(LandData setData)
    {
        _currentData = setData;
    }


    // Interactive Functions
    public void Place_CurrentLand()
    {
        MainController main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();

        // set current tile to snappoint
        Cursor cursor = main.cursor;
        Land_SnapPoint snapPoint = cursor.hoveringObject.GetComponent<Land_SnapPoint>();

        snapPoint.currentData.Update_CurrentLand(this);
        Instantiate(gameObject, snapPoint.transform.position, Quaternion.identity);

        // cursor update
        cursor.Clear_Card();
    }
}