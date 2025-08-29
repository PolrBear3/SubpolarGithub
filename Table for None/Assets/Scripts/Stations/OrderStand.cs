using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderStand : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Station_Controller _stationController;

    [Space(20)]
    [SerializeField] private Clock_Timer _coolTimer;
    [SerializeField] private AmountBar _npcCountBar;
    
    [Space(20)]
    [SerializeField] private Sprite[] _toggleSprites;
    
    [Space(20)]
    [SerializeField] private SpriteRenderer _orderingArea;

    [Space(20)]
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
        
        _stationController.detection.EnterEvent += Toggle_Indications;
    }

    private void OnDestroy()
    {
        // subscriptions
        ActionBubble_Interactable interactable = _stationController.interactable;

        interactable.OnAction1 -= Toggle_Activation;
        interactable.OnAction1 -= Toggle_Sprite;

        interactable.OnInteract -= Toggle_Indications;
        interactable.OnUnInteract -= Toggle_Indications;

        _stationController.detection.EnterEvent -= Toggle_Indications;
    }


    // Indications
    private void Toggle_Sprite()
    {
        Sprite mainSprite = _coroutine == null ? _toggleSprites[0] : _toggleSprites[1];
        _stationController.spriteRenderer.sprite = mainSprite;
        
        Action_Bubble bubble = _stationController.interactable.bubble;
        ActionBubble_Data bubbleData = _coroutine == null ? bubble.bubbleDatas[1] : bubble.bubbleDatas[0];

        bubble.Set_Bubble(bubbleData.iconSprite, null);
        bubble.Set_IndicatorToggleDatas(new List<ActionBubble_Data>() { bubbleData });
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
        Location_Controller currentLocation = Main_Controller.instance.currentLocation;
        SpriteRenderer vehicleArea = Main_Controller.instance.currentVehicle.interactArea;
        
        List<NPC_Controller> foodOrderNPCs = currentLocation.foodOrderNPCs;
        List<NPC_Controller> orderNPCs = new();

        for (int i = 0; i < foodOrderNPCs.Count; i++)
        {
            NPC_Controller npc = foodOrderNPCs[i];

            if (npc.movement.roamActive == false) continue;
            if (npc.movement.currentRoamArea != vehicleArea) continue;
            if (npc.foodInteraction == null) continue;
            if (npc.foodInteraction.timeCoroutine == null) continue;

            orderNPCs.Add(npc);
        }

        // Prioritize NPCs with conditions
        orderNPCs.Sort((a, b) =>
        {
            int aHasCond = a.foodIcon.currentData.conditionDatas.Count > 0 ? 0 : 1;
            int bHasCond = b.foodIcon.currentData.conditionDatas.Count > 0 ? 0 : 1;

            return aHasCond.CompareTo(bHasCond);
        });

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
                FoodOrder_NPCs()[i].movement.CurrentLocation_FreeRoam(_orderingArea, 0f);

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
        foreach (NPC_Controller npc in _currentNPCs)
        {
            npc.foodInteraction.Update_RoamArea();
        }
        _currentNPCs.Clear();

        _npcCountBar.Load_Custom(_maxCount, _currentNPCs.Count);
    }
}