using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CraftNPC_Mechanic : CraftNPC
{
    [Header("")]
    [SerializeField] private GameObject _toolBox;


    [Header("")]
    [SerializeField] private Vector2[] _interactRanges;

    [SerializeField][Range(0, 1)] private float _moveSpeedValue;

    private int _recentMenuNum;


    private ActionSelector _droppedToolBox;


    // MonoBehaviour
    private void Awake()
    {
        Load_Data();
    }

    private new void Start()
    {
        base.Start();
        Subscribe_OnSave(Save_Data);

        // subscriptions
        npcController.mainController.currentVehicle.menu.MenuOpen_Event += Update_RecentMenuNum;

        GlobalTime_Controller.TimeTik_Update += Set_ToolBox;
        GlobalTime_Controller.TimeTik_Update += Collect_ToolBox;

        ActionBubble_Interactable interactable = npcController.interactable;

        interactable.OnIInteract += Update_ActionBubble;
        interactable.OnAction1Input += Purchase;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // subscriptions
        npcController.mainController.currentVehicle.menu.MenuOpen_Event += Update_RecentMenuNum;

        GlobalTime_Controller.TimeTik_Update -= Set_ToolBox;
        GlobalTime_Controller.TimeTik_Update -= Collect_ToolBox;

        ActionBubble_Interactable interactable = npcController.interactable;

        interactable.OnIInteract -= Update_ActionBubble;
        interactable.OnAction1Input -= Purchase;
    }


    // Private Save and Load
    private void Save_Data()
    {
        ES3.Save("CraftNPC_Mechanic/nuggetBar.currentAmount", nuggetBar.currentAmount);
        ES3.Save("CraftNPC_Mechanic/npcController.foodIcon.AllDatas()", npcController.foodIcon.AllDatas());
    }

    private void Load_Data()
    {
        nuggetBar.Set_Amount(ES3.Load("CraftNPC_Mechanic/nuggetBar.currentAmount", nuggetBar.currentAmount));
        npcController.foodIcon.Update_AllDatas(ES3.Load("CraftNPC_Mechanic/npcController.foodIcon.AllDatas()", npcController.foodIcon.AllDatas()));
    }


    // Indications
    private void Update_ActionBubble()
    {
        Toggle_AmountBars();

        ActionBubble_Interactable interactable = npcController.interactable;
        Action_Bubble bubble = interactable.bubble;

        if (_droppedToolBox == null)
        {
            bubble.Toggle(false);
            bubble.Empty_Bubble();
            return;
        }

        bubble.Set_Bubble(_droppedToolBox.indicatorIcon.sprite, null);
    }


    // Basic Actions
    private void Drop_ToolBox()
    {
        if (_droppedToolBox != null) return;

        Vector2 dropPos = Main_Controller.SnapPosition(transform.position);

        if (npcController.mainController.Position_Claimed(dropPos)) return;

        GameObject drop = Instantiate(_toolBox, dropPos, quaternion.identity);
        drop.transform.SetParent(npcController.mainController.otherFile);

        _droppedToolBox = drop.GetComponent<ActionSelector>();

        // subscriptions
        _droppedToolBox.Subscribe_Action(Upgrade_MoveSpeed);
        _droppedToolBox.Subscribe_Action(Upgrade_InteractRange);
        _droppedToolBox.Subscribe_Action(Upgrade_StorageSpace);

        _droppedToolBox.OnActionToggle += Update_ActionBubble;
    }


    private bool ToolBox_SetAvailable()
    {
        Main_Controller main = npcController.mainController;
        Vehicle_Controller vehicle = main.currentVehicle;

        List<Vector2> surroundPositions = vehicle.positionClaimer.All_SurroundPositions();

        for (int i = 0; i < surroundPositions.Count; i++)
        {
            if (main.Position_Claimed(surroundPositions[i])) continue;
            return true;
        }

        return false;
    }

    private Vector2 ToolBox_SetPosition()
    {
        Main_Controller main = npcController.mainController;
        Vehicle_Controller vehicle = main.currentVehicle;

        List<Vector2> surroundPositions = vehicle.positionClaimer.All_SurroundPositions();

        for (int i = 0; i < surroundPositions.Count; i++)
        {
            if (main.Position_Claimed(surroundPositions[i])) continue;
            return surroundPositions[i];
        }

        return Vector2.zero;
    }

    private void Set_ToolBox()
    {
        if (coroutine != null) return;
        if (_droppedToolBox != null) return;

        if (ToolBox_SetAvailable() == false) return;

        Vehicle_Controller vehicle = npcController.mainController.currentVehicle;
        if (vehicle.movement.onBoard) return;

        Set_Coroutine(StartCoroutine(Set_ToolBox_Coroutine()));
    }
    private IEnumerator Set_ToolBox_Coroutine()
    {
        NPC_Movement movement = npcController.movement;
        Vector2 setPos = ToolBox_SetPosition();

        movement.Stop_FreeRoam();
        movement.Assign_TargetPosition(setPos);

        while (movement.At_TargetPosition(setPos) == false)
        {
            // cancel set action if interact during action
            if (movement.Is_Moving() == false)
            {
                Set_Coroutine(null);
                yield break;
            }

            yield return null;
        }

        if (npcController.mainController.Position_Claimed(setPos))
        {
            Set_Coroutine(null);
            Set_ToolBox();

            yield break;
        }

        Drop_ToolBox();

        movement.Free_Roam(0);

        Set_Coroutine(null);
        yield break;
    }


    private bool ToolBox_NearbyVehicle()
    {
        Vehicle_Controller vehicle = npcController.mainController.currentVehicle;
        List<Vector2> allPositions = vehicle.positionClaimer.All_SurroundPositions();

        for (int i = allPositions.Count - 1; i >= 0; i--)
        {
            if ((Vector2)_droppedToolBox.transform.position == allPositions[i]) return true;
        }

        return false;
    }

    private void Collect_ToolBox()
    {
        if (coroutine != null) return;

        if (_droppedToolBox == null) return;
        if (ToolBox_NearbyVehicle()) return;

        Set_Coroutine(StartCoroutine(Collect_ToolBox_Coroutine()));
    }
    private IEnumerator Collect_ToolBox_Coroutine()
    {
        NPC_Movement movement = npcController.movement;

        movement.Stop_FreeRoam();
        movement.Assign_TargetPosition(_droppedToolBox.transform.position);

        while (movement.At_TargetPosition(_droppedToolBox.transform.position) == false)
        {
            // cancel collect action if interact during action
            if (movement.Is_Moving() == false)
            {
                Set_Coroutine(null);
                yield break;
            }

            yield return null;
        }

        GameObject currentToolBox = _droppedToolBox.gameObject;
        _droppedToolBox = null;

        Destroy(currentToolBox);
        Update_ActionBubble();

        movement.Free_Roam(0);

        Set_Coroutine(null);
        yield break;
    }


    private void Purchase()
    {
        if (coroutine != null) return;
        if (_droppedToolBox == null) return;
        if (nuggetBar.Is_MaxAmount() == false) return;

        Set_Coroutine(StartCoroutine(Purchase_Coroutine()));
    }
    private IEnumerator Purchase_Coroutine()
    {
        npcController.interactable.LockInteract(true);

        NPC_Movement movement = npcController.movement;
        movement.Stop_FreeRoam();

        movement.Assign_TargetPosition(_droppedToolBox.transform.position);
        while (movement.At_TargetPosition() == false) yield return null;

        Vector2 vehicle = npcController.mainController.currentVehicle.transform.position;

        movement.Assign_TargetPosition(vehicle);
        while (movement.At_TargetPosition() == false) yield return null;

        _droppedToolBox.Invoke_Action();
        movement.Free_Roam(0);

        npcController.interactable.LockInteract(false);

        Set_Coroutine(null);
        yield break;
    }


    // Purchase Upgrades
    private void Upgrade_InteractRange()
    {
        Vehicle_Controller vehicle = npcController.mainController.currentVehicle;
        Vector2 currentRange = vehicle.interactArea.size;

        for (int i = 0; i < _interactRanges.Length; i++)
        {
            if (_interactRanges[i] != currentRange) continue;
            if (i >= _interactRanges.Length - 1) break;

            vehicle.Update_InteractArea_Range(_interactRanges[i + 1]);

            nuggetBar.Set_Amount(0);
            nuggetBar.Load();

            return;
        }

        nuggetBar.Set_Amount(nuggetBar.maxAmount);
        nuggetBar.Load();

        // dialog //
    }

    private void Upgrade_MoveSpeed()
    {
        VehicleMovement_Controller vehicle = npcController.mainController.currentVehicle.movement;

        if (vehicle.moveSpeed >= vehicle.maxMoveSpeed)
        {
            // dialog //
            return;
        }

        vehicle.Update_MovementSpeed(_moveSpeedValue);

        nuggetBar.Set_Amount(0);
        nuggetBar.Load();
    }


    private void Update_RecentMenuNum()
    {
        VehicleMenu_Controller vehicle = npcController.mainController.currentVehicle.menu;
        GameObject recentMenu = vehicle.menus[vehicle.currentMenuNum];

        bool foodMenuOpened = recentMenu.TryGetComponent<FoodMenu_Controller>(out _);
        bool stationMenuOpened = recentMenu.TryGetComponent<StationMenu_Controller>(out _);

        if (foodMenuOpened == false && stationMenuOpened == false) return;

        _recentMenuNum = vehicle.currentMenuNum;
    }

    private void Upgrade_StorageSpace()
    {
        VehicleMenu_Controller menuController = npcController.mainController.currentVehicle.menu;
        ItemSlots_Controller slotsController = menuController.slotsController;

        GameObject recentMenu = menuController.menus[_recentMenuNum];
        Dictionary<int, List<ItemSlot_Data>> slotDatas = recentMenu.GetComponent<IVehicleMenu>().ItemSlot_Datas();

        if (slotDatas.Count >= slotsController.maxPageNum)
        {
            // dialog //
            return;
        }

        slotsController.AddNewPage_ItemSlotDatas(slotDatas);

        nuggetBar.Set_Amount(0);
        nuggetBar.Load();

        if (recentMenu.activeSelf == false) return;

        menuController.Update_PageDots(slotDatas.Count);
    }
}