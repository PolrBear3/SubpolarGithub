using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderStand : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Station_Controller _stationController;

    [Header("")]
    [SerializeField] private Clock_Timer _coolTimer;
    [SerializeField] private AmountBar _npcCountBar;


    [Header("")]
    [SerializeField] private Sprite[] _toggleSprites;
    [SerializeField] private Sprite[] _bubbleSprites;


    [Header("")]
    [SerializeField] private SpriteRenderer _orderingArea;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _searchTime;
    [SerializeField][Range(0, 100)] private int _maxCount;


    private List<NPC_Controller> _currentNPCs = new();

    private Coroutine _coroutine;


    // UnityEngine
    private void Start()
    {
        Toggle_Sprite();

        // subscriptions
        ActionBubble_Interactable interactable = _stationController.interactable;

        interactable.OnAction1 += Toggle_Activation;
        interactable.OnAction1 += Toggle_Sprite;

        interactable.OnInteract += Toggle_Indications;
        interactable.OnUnInteract += Toggle_Indications;

        Detection_Controller detection = _stationController.detection;

        detection.EnterEvent += Toggle_Indications;
    }

    private void OnDestroy()
    {
        // subscriptions
        ActionBubble_Interactable interactable = _stationController.interactable;

        interactable.OnAction1 -= Toggle_Activation;
        interactable.OnAction1 -= Toggle_Sprite;

        interactable.OnInteract -= Toggle_Indications;
        interactable.OnUnInteract -= Toggle_Indications;

        Detection_Controller detection = _stationController.detection;

        detection.EnterEvent -= Toggle_Indications;
    }


    // Indications
    private void Toggle_Sprite()
    {
        Action_Bubble bubble = _stationController.interactable.bubble;

        if (_coroutine == null)
        {
            _stationController.spriteRenderer.sprite = _toggleSprites[0];
            bubble.Set_Bubble(_bubbleSprites[1], null);

            return;
        }

        _stationController.spriteRenderer.sprite = _toggleSprites[1];
        bubble.Set_Bubble(_bubbleSprites[0], null);
    }

    private void Toggle_Indications()
    {
        Action_Bubble bubble = _stationController.interactable.bubble;

        bool playerDetected = _stationController.detection.player != null;
        bool toggleOn = bubble.bubbleOn == false && playerDetected && _coroutine != null;

        _coolTimer.Toggle_Transparency(!toggleOn);
        _npcCountBar.Toggle(toggleOn);
    }


    // NPC
    private void Toggle_Activation()
    {
        if (_coroutine == null)
        {
            Update_RoamArea();
            return;
        }

        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = null;

        Reset_CurrentNPCs();

        _coolTimer.Stop_Time();
        _coolTimer.Toggle_Transparency(true);
    }


    /// <returns>
    /// All npcs with food order & time running & at vehicle area
    /// </returns>
    private List<NPC_Controller> FoodOrder_NPCs()
    {
        Main_Controller main = Main_Controller.instance;
        SpriteRenderer vehicleArea = main.currentVehicle.interactArea;

        List<NPC_Controller> allNPCs = main.All_NPCs();
        List<NPC_Controller> orderNPCs = new();

        for (int i = 0; i < allNPCs.Count; i++)
        {
            if (allNPCs[i].movement.roamActive == false) continue;
            if (allNPCs[i].movement.currentRoamArea != vehicleArea) continue;

            if (allNPCs[i].gameObject.TryGetComponent(out NPC_FoodInteraction foodInteract) == false) continue;
            if (foodInteract.timeCoroutine == null) continue;

            orderNPCs.Add(allNPCs[i]);
        }

        return orderNPCs;
    }

    private void Update_RoamArea()
    {
        _coroutine = StartCoroutine(Update_RoamArea_Coroutine());
    }
    private IEnumerator Update_RoamArea_Coroutine()
    {
        while (true)
        {
            _coolTimer.Set_Time(_searchTime);
            _coolTimer.Run_Time();

            yield return new WaitForSeconds(_searchTime);

            Refresh_CurrentNPCs();

            for (int i = 0; i < FoodOrder_NPCs().Count; i++)
            {
                if (_currentNPCs.Count >= _maxCount) continue;
                if (_currentNPCs.Contains(FoodOrder_NPCs()[i])) continue;

                _currentNPCs.Add(FoodOrder_NPCs()[i]);
                FoodOrder_NPCs()[i].movement.Free_Roam(_orderingArea, 0f);

                break;
            }

            _npcCountBar.Load_Custom(_maxCount, _currentNPCs.Count);
        }
    }


    private void Refresh_CurrentNPCs()
    {
        for (int i = _currentNPCs.Count - 1; i >= 0; i--)
        {
            bool atCurrentArea = _currentNPCs[i].movement.currentRoamArea == _orderingArea;
            bool timeRunning = _currentNPCs[i].foodInteraction.timeCoroutine != null;

            if (atCurrentArea && timeRunning) continue;
            _currentNPCs.RemoveAt(i);
        }
    }

    private void Reset_CurrentNPCs()
    {
        Main_Controller main = Main_Controller.instance;

        SpriteRenderer vehicleArea = main.currentVehicle.interactArea;
        SpriteRenderer locationArea = main.currentLocation.data.roamArea;

        for (int i = 0; i < _currentNPCs.Count; i++)
        {
            if (_currentNPCs[i].foodInteraction.timeCoroutine != null)
            {
                _currentNPCs[i].movement.Free_Roam(vehicleArea, 0f);
                continue;
            }
            _currentNPCs[i].movement.Free_Roam(locationArea, 0f);
        }
        _currentNPCs.Clear();

        _npcCountBar.Load_Custom(_maxCount, _currentNPCs.Count);
    }
}