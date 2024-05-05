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

        _main.NextTurn += Activate_LandBuffs;
    }

    private void OnDestroy()
    {
        _main.NextTurn -= Activate_LandBuffs;
    }


    // ISnapPointInteractable and EventTrigger
    public void Interact()
    {
        Place_CurrentLand();
    }

    public void OnPointerClick()
    {
        Cursor cursor = _main.cursor;

        // if card not dragging, return
        if (!cursor.isDragging) return;

        // get cursor gameobject > get ISnapPointInteractable
        if (!cursor.dragCardGameObject.TryGetComponent(out ILandInteractable interactable)) return;

        interactable.Interact();

        // card
        cursor.dragCard.Use();

        // cursor
        cursor.Clear_Card();
    }

    public void OnPointerEnter()
    {
        _main.cursor.Update_HoverObject(gameObject);
    }

    public void OnPointerExit()
    {
        _main.cursor.Update_HoverObject(null);
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

        // deactivate pointer enter exit event for snappoint
        snapPoint.BoxCollider_Toggle(false);

        // set land on snappoint
        GameObject land = Instantiate(gameObject, snapPoint.transform.position, Quaternion.identity);
        land.transform.SetParent(snapPoint.transform);
    }


    // Next Turn Events
    private void Activate_LandBuffs()
    {
        for (int i = 0; i < _currentData.landBuffPrefabs.Count; i++)
        {
            if (!_currentData.landBuffPrefabs[i].TryGetComponent(out ILandInteractable interactable)) continue;
            interactable.Interact();
        }

        _currentData.Clear_LandBuffs();
    }
}