using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CraftNPC_Mechanic : CraftNPC
{
    [Header("")]
    [SerializeField] private GameObject _toolBox;

    private ActionSelector _droppedToolBox;


    [Header("")]
    [SerializeField][Range(0, 100)] private float _upgradeTimeValue;


    [Header("")]
    // interact range upgradge
    [SerializeField] private Vector2[] _interactRanges;

    // vehicle movement speed upgrade
    [SerializeField][Range(0, 1)] private float _speedUpgradeValue;

    // vehicle menu slot page upgrade
    private int _recentMenuNum;




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
        Main_Controller.instance.currentVehicle.menu.MenuOpen_Event += Update_RecentMenuNum;

        GlobalTime_Controller.instance.OnTimeTik += Set_ToolBox;
        GlobalTime_Controller.instance.OnTimeTik += Collect_ToolBox;

        ActionBubble_Interactable interactable = main.interactable;

        interactable.OnHoldIInteract += Set_ToolBox;

        interactable.OnIInteract += Update_ActionBubble;
        interactable.OnAction1Input += Purchase;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // subscriptions
        Main_Controller.instance.currentVehicle.menu.MenuOpen_Event += Update_RecentMenuNum;

        GlobalTime_Controller.instance.OnTimeTik -= Set_ToolBox;
        GlobalTime_Controller.instance.OnTimeTik -= Collect_ToolBox;

        ActionBubble_Interactable interactable = main.interactable;

        interactable.OnHoldIInteract -= Set_ToolBox;

        interactable.OnIInteract -= Update_ActionBubble;
        interactable.OnAction1Input -= Purchase;
    }


    // Private Save and Load
    private void Save_Data()
    {
        ES3.Save("CraftNPC_Mechanic/nuggetBar.currentAmount", nuggetBar.currentAmount);
        ES3.Save("CraftNPC_Mechanic/npcController.foodIcon.AllDatas()", main.foodIcon.AllDatas());
    }

    private void Load_Data()
    {
        nuggetBar.Set_Amount(ES3.Load("CraftNPC_Mechanic/nuggetBar.currentAmount", nuggetBar.currentAmount));
        main.foodIcon.Update_AllDatas(ES3.Load("CraftNPC_Mechanic/npcController.foodIcon.AllDatas()", main.foodIcon.AllDatas()));
    }


    // Indications
    private void Update_ActionBubble()
    {
        Toggle_AmountBars();

        ActionBubble_Interactable interactable = main.interactable;
        Action_Bubble bubble = interactable.bubble;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_droppedToolBox == null)
        {
            bubble.Toggle(false);
            bubble.Empty_Bubble();

            dialog.Update_Dialog(Current_PayCount());

            return;
        }

        bubble.Set_Bubble(_droppedToolBox.indicatorIcon.sprite, null);

        if (bubble.bubbleOn == false) return;

        dialog.Update_Dialog(_droppedToolBox.currentIndex);
    }


    // Basic Actions
    private void Drop_ToolBox()
    {
        if (_droppedToolBox != null) return;

        Main_Controller main = Main_Controller.instance;
        Vector2 dropPos = main.SnapPosition(transform.position);

        if (main.Position_Claimed(dropPos)) return;

        GameObject drop = Instantiate(_toolBox, dropPos, quaternion.identity);
        drop.transform.SetParent(main.otherFile);

        _droppedToolBox = drop.GetComponent<ActionSelector>();

        // subscriptions
        _droppedToolBox.Subscribe_Action(Upgrade_MoveSpeed);
        _droppedToolBox.Subscribe_Action(Upgrade_InteractRange);
        _droppedToolBox.Subscribe_Action(Upgrade_StorageSpace);

        _droppedToolBox.OnActionToggle += Update_ActionBubble;
    }


    private bool ToolBox_SetAvailable()
    {
        if (coroutine != null) return false;
        if (_droppedToolBox != null) return false;

        if (nuggetBar.Is_MaxAmount() == false) return false;

        Main_Controller main = Main_Controller.instance;

        Location_Controller location = main.currentLocation;
        Vehicle_Controller vehicle = main.currentVehicle;

        List<Vector2> surroundPositions = vehicle.positionClaimer.All_SurroundPositions();

        for (int i = 0; i < surroundPositions.Count; i++)
        {
            if (main.Position_Claimed(surroundPositions[i])) continue;
            if (location.Restricted_Position(surroundPositions[i])) continue;

            return true;
        }

        return false;
    }

    private Vector2 ToolBox_SetPosition()
    {
        Main_Controller main = Main_Controller.instance;
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
        if (ToolBox_SetAvailable() == false) return;

        Vehicle_Controller vehicle = Main_Controller.instance.currentVehicle;
        if (vehicle.movement.onBoard) return;

        Set_Coroutine(StartCoroutine(Set_ToolBox_Coroutine()));

        actionTimer.Toggle_RunAnimation(true);
        main.interactable.LockInteract(true);

        Toggle_AmountBars();
    }
    private IEnumerator Set_ToolBox_Coroutine()
    {
        NPC_Movement movement = main.movement;
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

        if (Main_Controller.instance.Position_Claimed(setPos))
        {
            Set_ToolBox();

            Set_Coroutine(null);
            yield break;
        }

        Drop_ToolBox();
        movement.Free_Roam(0);

        actionTimer.Toggle_RunAnimation(false);
        main.interactable.LockInteract(false);

        Set_Coroutine(null);
        Toggle_AmountBars();

        yield break;
    }


    private bool ToolBox_NearbyVehicle()
    {
        Vehicle_Controller vehicle = Main_Controller.instance.currentVehicle;
        List<Vector2> allPositions = vehicle.positionClaimer.All_SurroundPositions();

        for (int i = allPositions.Count - 1; i >= 0; i--)
        {
            if ((Vector2)_droppedToolBox.transform.position == allPositions[i]) return true;
        }

        return false;
    }

    private void ToolBox_Collect()
    {
        if (_droppedToolBox == null) return;

        GameObject currentToolBox = _droppedToolBox.gameObject;
        _droppedToolBox = null;

        Destroy(currentToolBox);
        Update_ActionBubble();
    }

    private void Collect_ToolBox()
    {
        if (coroutine != null) return;

        if (_droppedToolBox == null) return;
        if (ToolBox_NearbyVehicle()) return;

        Set_Coroutine(StartCoroutine(Collect_ToolBox_Coroutine()));

        actionTimer.Toggle_RunAnimation(true);
        main.interactable.LockInteract(true);

        Toggle_AmountBars();
    }
    private IEnumerator Collect_ToolBox_Coroutine()
    {
        NPC_Movement movement = main.movement;

        movement.Stop_FreeRoam();
        movement.Assign_TargetPosition(_droppedToolBox.transform.position);

        while (movement.At_TargetPosition(_droppedToolBox.transform.position) == false) yield return null;

        ToolBox_Collect();
        movement.Free_Roam(0);

        actionTimer.Toggle_RunAnimation(false);
        main.interactable.LockInteract(false);

        Set_Coroutine(null);
        Toggle_AmountBars();

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
        actionTimer.Toggle_RunAnimation(true);
        main.interactable.LockInteract(true);

        Toggle_AmountBars();

        NPC_Movement movement = main.movement;
        movement.Stop_FreeRoam();

        // move to tool box
        movement.Assign_TargetPosition(_droppedToolBox.transform.position);
        while (movement.At_TargetPosition() == false) yield return null;

        Vehicle_Controller vehicle = Main_Controller.instance.currentVehicle;
        Vector2 vehiclePosition = vehicle.transform.position;

        // move to vehicle
        movement.Assign_TargetPosition(vehiclePosition);
        while (movement.At_TargetPosition() == false) yield return null;

        // upgrade time delay
        yield return new WaitForSeconds(_upgradeTimeValue);

        // upgrade
        _droppedToolBox.Invoke_Action();

        vehicle.SilverShine_VehicleBody();

        // collect tool box
        movement.Assign_TargetPosition(_droppedToolBox.transform.position);
        while (movement.At_TargetPosition() == false) yield return null;

        ToolBox_Collect();

        // end action
        movement.Free_Roam(0);

        actionTimer.Toggle_RunAnimation(false);
        main.interactable.LockInteract(false);

        Set_Coroutine(null);
        Toggle_AmountBars();

        yield break;
    }


    // Purchase Upgrades
    private void Upgrade_InteractRange()
    {
        Vehicle_Controller vehicle = Main_Controller.instance.currentVehicle;
        Vector2 currentRange = vehicle.interactArea.size;

        for (int i = 0; i < _interactRanges.Length; i++)
        {
            if (_interactRanges[i] != currentRange) continue;
            if (i >= _interactRanges.Length - 1) break;

            vehicle.Update_InteractArea_Range(_interactRanges[i + 1]);

            nuggetBar.Set_Amount(0);
            nuggetBar.Toggle_BarColor(false);
            nuggetBar.Load();

            return;
        }

        // upgrade fail dialog //
    }


    private void Upgrade_MoveSpeed()
    {
        VehicleMovement_Controller vehicle = Main_Controller.instance.currentVehicle.movement;

        if (vehicle.moveSpeed >= vehicle.maxMoveSpeed)
        {
            // upgrade fail dialog //
            return;
        }

        vehicle.Update_MovementSpeed(_speedUpgradeValue);

        nuggetBar.Set_Amount(0);
        nuggetBar.Toggle_BarColor(false);
        nuggetBar.Load();
    }


    private void Update_RecentMenuNum()
    {
        VehicleMenu_Controller vehicle = Main_Controller.instance.currentVehicle.menu;
        GameObject recentMenu = vehicle.menus[vehicle.currentMenuNum];

        bool foodMenuOpened = recentMenu.TryGetComponent<FoodMenu_Controller>(out _);
        bool stationMenuOpened = recentMenu.TryGetComponent<StationMenu_Controller>(out _);

        if (foodMenuOpened == false && stationMenuOpened == false) return;

        _recentMenuNum = vehicle.currentMenuNum;
    }

    private void Upgrade_StorageSpace()
    {
        VehicleMenu_Controller menuController = Main_Controller.instance.currentVehicle.menu;
        ItemSlots_Controller slotsController = menuController.slotsController;

        GameObject recentMenu = menuController.menus[_recentMenuNum];
        Dictionary<int, List<ItemSlot_Data>> slotDatas = recentMenu.GetComponent<IVehicleMenu>().ItemSlot_Datas();

        if (slotDatas.Count >= slotsController.maxPageNum)
        {
            // upgrade fail dialog //
            return;
        }

        slotsController.AddNewPage_ItemSlotDatas(slotDatas);

        nuggetBar.Set_Amount(0);
        nuggetBar.Toggle_BarColor(false);
        nuggetBar.Load();

        if (recentMenu.activeSelf == false) return;

        menuController.Update_PageDots(slotDatas.Count);
    }
}