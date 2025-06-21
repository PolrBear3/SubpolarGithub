using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CraftNPC_Mechanic : CraftNPC
{
    [Space(60)]
    [SerializeField] private Sprite _toolBoxSprite;
    [SerializeField] private GameObject _toolBox;
    
    [Space(20)]
    [SerializeField] private ActionSelector_Data[] _actionDatas;
    
    [Space(20)]
    [SerializeField] private Vector2[] _interactRanges;
    [SerializeField][Range(0, 1)] private float _speedUpgradeValue;

    
    private ActionSelector _droppedToolBox;
    private int _recentMenuNum;


    // MonoBehaviour
    private new void Awake()
    {
        base.Awake();
        Load_Data();
    }

    private new void Start()
    {
        base.Start();
        Subscribe_OnSave(Save_Data);

        // subscriptions
        Main_Controller.instance.currentVehicle.menu.On_MenuToggle += Update_RecentMenuNum;

        globaltime.instance.OnTimeTik += Set_ToolBox;
        globaltime.instance.OnTimeTik += Collect_ToolBox;

        ActionBubble_Interactable interactable = npcController.interactable;

        interactable.OnInteract += Toggle_ActionBubble;
        interactable.OnInteract += Toggle_PurchasePrice;

        interactable.OnAction1 += Purchase;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
        
        ToolBox_Collect();

        // subscriptions
        Main_Controller.instance.currentVehicle.menu.On_MenuToggle += Update_RecentMenuNum;

        globaltime.instance.OnTimeTik -= Set_ToolBox;
        globaltime.instance.OnTimeTik -= Collect_ToolBox;

        ActionBubble_Interactable interactable = npcController.interactable;

        interactable.OnInteract -= Toggle_ActionBubble;
        interactable.OnInteract -= Toggle_PurchasePrice;

        interactable.OnAction1 -= Purchase;
    }


    // Private Save and Load
    private void Save_Data()
    {
        // ES3.Save("CraftNPC_Mechanic/CraftNPC_Data", data);
        ES3.Save("CraftNPC_Mechanic/PurchaseData", purchaseData);
    }

    private void Load_Data()
    {
        // Set_Data(ES3.Load("CraftNPC_Mechanic/CraftNPC_Data"));
        Set_PurchaseData(ES3.Load("CraftNPC_Mechanic/PurchaseData", new PurchaseData(defaultPrice)));
    }


    // Indications
    private void Toggle_ActionBubble()
    {
        ActionBubble_Interactable interactable = npcController.interactable;
        Action_Bubble bubble = interactable.bubble;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_droppedToolBox == null)
        {
            bubble.Toggle(false);
            bubble.Empty_Bubble();

            return;
        }
        
        bubble.Set_Bubble(_droppedToolBox.indicatorIcon.sprite, null);
        
        if (bubble.bubbleOn == false) return;
        dialog.Update_Dialog(_droppedToolBox.currentIndex);
    }

    private void Toggle_PurchasePrice()
    {
        GoldSystem system = GoldSystem.instance;

        if (_toolBox == null || purchaseData.purchased == false)
        {
            system.Indicate_TriggerData(new(npcIconSprite, -Purchase_Price()));
            return;
        }

        // additional price indication //
    }


    // Tool Box
    private void Drop_ToolBox()
    {
        if (_droppedToolBox != null) return;

        Main_Controller main = Main_Controller.instance;
        Vector2 dropPos = main.SnapPosition(transform.position);

        if (main.Position_Claimed(dropPos)) return;

        GameObject drop = Instantiate(_toolBox, dropPos, quaternion.identity);
        drop.transform.SetParent(main.otherFile);

        _droppedToolBox = drop.GetComponent<ActionSelector>();
        _droppedToolBox.sr.sprite = _toolBoxSprite;

        foreach (ActionSelector_Data data in _actionDatas)
        {
            _droppedToolBox.Add_ActionData(data);
        }
        
        npcController.interactable.bubble.Set_Bubble(_droppedToolBox.indicatorIcon.sprite, null);
    }


    private bool ToolBox_SetAvailable()
    {
        if (coroutine != null) return false;
        if (_droppedToolBox != null) return false;
        if (purchaseData.purchased == false) return false;

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
    }
    private IEnumerator Set_ToolBox_Coroutine()
    {
        Toggle_Action(true);

        NPC_Movement movement = npcController.movement;
        
        Vector2 setPos = ToolBox_SetPosition();
        movement.Assign_TargetPosition(setPos);

        while (movement.At_TargetPosition(setPos) == false) yield return null;

        if (Main_Controller.instance.Position_Claimed(setPos))
        {
            Set_ToolBox();

            Toggle_Action(false);
            yield break;
        }

        Drop_ToolBox();

        Toggle_Action(false);
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
        Toggle_ActionBubble();
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
        Toggle_Action(true);

        NPC_Movement movement = npcController.movement;
        movement.Assign_TargetPosition(_droppedToolBox.transform.position);

        while (movement.At_TargetPosition(_droppedToolBox.transform.position) == false) yield return null;

        ToolBox_Collect();

        Toggle_Action(false);
        yield break;
    }


    // Purchase
    private int Purchase_Price()
    {
        if (_droppedToolBox != null) return 0;

        return defaultPrice;
    }

    private void Purchase()
    {
        if (coroutine != null) return;
        if (_droppedToolBox == null) return;

        Set_Coroutine(StartCoroutine(Purchase_Coroutine()));
    }
    private IEnumerator Purchase_Coroutine()
    {
        Toggle_Action(true);

        NPC_Movement movement = npcController.movement;

        // move to tool box
        movement.Assign_TargetPosition(_droppedToolBox.transform.position);
        while (movement.At_TargetPosition() == false) yield return null;

        Vehicle_Controller vehicle = Main_Controller.instance.currentVehicle;
        VehicleMovement_Controller vehicleMovement = vehicle.movement;

        // move to vehicle
        Vector2 vehiclePosition = vehicle.transform.position;
        movement.Assign_TargetPosition(vehiclePosition);

        while (movement.At_TargetPosition() == false)
        {
            // cancel purchase if player boards on vehicle
            if (vehicleMovement.onBoard)
            {
                Toggle_Action(false);
                yield break;
            }

            yield return null;
        }

        // upgrade time delay
        yield return new WaitForSeconds(upgradeTimeValue);

        // upgrade
        _droppedToolBox.Invoke_Action();

        // collect tool box
        movement.Assign_TargetPosition(_droppedToolBox.transform.position);
        while (movement.At_TargetPosition() == false) yield return null;

        ToolBox_Collect();

        // data update
        Set_PurchaseData(new(defaultPrice));

        Toggle_Action(false);
        yield break;
    }


    // ActionSelector_Data
    public void Upgrade_InteractRange()
    {
        Vehicle_Controller vehicle = Main_Controller.instance.currentVehicle;
        Vector2 currentRange = vehicle.interactArea.size;

        for (int i = 0; i < _interactRanges.Length; i++)
        {
            if (_interactRanges[i] != currentRange) continue;
            if (i >= _interactRanges.Length - 1) break;

            vehicle.Update_InteractArea_Range(_interactRanges[i + 1]);

            /*
            nuggetBar.Set_Amount(0);
            nuggetBar.Toggle_BarColor(false);
            nuggetBar.Load();
            */

            return;
        }

        // upgrade fail dialog //
    }


    public void Upgrade_MoveSpeed()
    {
        VehicleMovement_Controller vehicle = Main_Controller.instance.currentVehicle.movement;

        if (vehicle.moveSpeed >= vehicle.maxMoveSpeed)
        {
            // upgrade fail dialog //
            return;
        }

        vehicle.Update_MovementSpeed(_speedUpgradeValue);

        /*
        nuggetBar.Set_Amount(0);
        nuggetBar.Toggle_BarColor(false);
        nuggetBar.Load();
        */
    }


    private void Update_RecentMenuNum(bool menuToggle)
    {
        if (menuToggle == false) return;

        VehicleMenu_Controller vehicle = Main_Controller.instance.currentVehicle.menu;
        GameObject recentMenu = vehicle.menus[vehicle.currentMenuNum];

        bool foodMenuOpened = recentMenu.TryGetComponent<FoodMenu_Controller>(out _);
        bool stationMenuOpened = recentMenu.TryGetComponent<StationMenu_Controller>(out _);

        if (foodMenuOpened == false && stationMenuOpened == false) return;

        _recentMenuNum = vehicle.currentMenuNum;
    }

    public void Upgrade_StorageSpace()
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

        if (recentMenu.activeSelf == false) return;

        menuController.Update_PageDots(slotDatas.Count);
        menuController.Update_PageArrows();
    }
}