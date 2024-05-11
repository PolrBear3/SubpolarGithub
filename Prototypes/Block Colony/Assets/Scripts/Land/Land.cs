using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILandInteractable
{
    void Interact();
}

public interface ILandEventable
{
    void Activate();
}

public class Land : MonoBehaviour, ISnapPointInteractable, IInteractCheck
{
    private MainController _main;
    public MainController main => _main;

    [SerializeField] private LandData _currentData;
    public LandData currentData => _currentData;

    [SerializeField] private LandEvents _events;
    public LandEvents events => _events;


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();
    }


    // ISnapPointInteractable
    public void Interact()
    {
        Place_CurrentLand();
    }

    public bool InteractAvailable()
    {
        MainController main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();

        Cursor cursor = main.cursor;
        Land_SnapPoint cursorSnapPoint = cursor.hoveringObject.GetComponent<Land_SnapPoint>();

        // check if there are surrounding lands on current snappoint
        if (main.CrossSurrounding_Lands(cursorSnapPoint).Count <= 0) return false;

        return true;
    }


    // EventTrigger
    public void OnPointerClick()
    {
        if (MainController.actionAvailable == false) return;

        Cursor cursor = _main.cursor;

        // if card not dragging, return
        if (!cursor.isDragging) return;

        // activate buff interactable
        if (!cursor.dragCardGameObject.TryGetComponent(out ILandInteractable interactable))
        {
            cursor.dragCard.Return();
            cursor.Clear_Card();
            return;
        }

        // check if interact available
        if (cursor.dragCardGameObject.TryGetComponent(out IInteractCheck check))
        {
            if (check.InteractAvailable() == false)
            {
                cursor.dragCard.Return();
                cursor.Clear_Card();
                return;
            }
        }

        // update order layer
        if (cursor.dragCardGameObject.TryGetComponent(out SpriteRenderer sr)) sr.sortingOrder -= 2;

        interactable.Interact();

        cursor.dragCard.Use();
        cursor.Clear_Card();
    }

    public void OnPointerEnter()
    {
        // cursor
        _main.cursor.Update_HoverObject(gameObject);
    }

    public void OnPointerExit()
    {
        // cursor
        _main.cursor.Update_HoverObject(null);
    }


    // Data Control
    public void Set_CurrentData(LandData setData)
    {
        _currentData = setData;
    }


    // ToolTip Current Events Description
    public string CurrentEvents_Description()
    {
        List<EventScrObj> nonDuplicateEvents = new();

        for (int i = 0; i < _currentData.currentEvents.Count; i++)
        {
            if (nonDuplicateEvents.Contains(_currentData.currentEvents[i])) continue;
            nonDuplicateEvents.Add(_currentData.currentEvents[i]);
        }

        List<string> allDescriptions = new();

        // current population
        allDescriptions.Add("<sprite=3>" + _currentData.population.ToString());

        for (int i = 0; i < nonDuplicateEvents.Count; i++)
        {
            allDescriptions.Add(_currentData.currentEvents[i].description + "  (x" + _currentData.Event_Count(nonDuplicateEvents[i]) + ")");
        }

        return string.Join("\n\n", allDescriptions);
    }


    // Interactive Functions
    public void Place_CurrentLand()
    {
        MainController main = GameObject.FindGameObjectWithTag("MainController").GetComponent<MainController>();

        Cursor cursor = main.cursor;
        Land setLand = cursor.dragCardGameObject.GetComponent<Land>();

        Land_SnapPoint snapPoint = cursor.hoveringObject.GetComponent<Land_SnapPoint>();

        // set land on snappoint
        GameObject spawn = Instantiate(setLand.gameObject, snapPoint.transform.position, Quaternion.identity);
        Land spawnLand = spawn.GetComponent<Land>();

        spawnLand.Set_CurrentData(new LandData(snapPoint, _currentData.type));
        snapPoint.currentData.Update_CurrentLand(spawnLand);

        spawn.transform.SetParent(snapPoint.transform);

        // current population update
        _main.Update_UpdatePopulation();
    }

    public void Remove_CurrentLand()
    {
        // reset snappoint data
        _currentData.snapPoint.currentData.Update_CurrentLand(null);

        // remove
        Destroy(gameObject);
    }
}