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


    // MonoBehaviour
    private void Awake()
    {
        Load_Data();
    }

    private new void Start()
    {
        base.Start();
        Subscribe_OnSave(Save_Data);

        Set_ToolBox();

        // subscriptions
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
        _droppedToolBox.Subscribe_Action(Upgrade_ExportRange);

        _droppedToolBox.OnActionToggle += Update_ActionBubble;
    }


    private Vector2 ToolBox_SetPosition()
    {
        Main_Controller main = npcController.mainController;
        Vehicle_Controller vehicle = main.currentVehicle;

        List<Vector2> allPositions = vehicle.positionClaimer.All_Positions();

        for (int i = allPositions.Count - 1; i >= 0; i--)
        {
            if (main.Position_Claimed(allPositions[i])) continue;
            return allPositions[i];
        }

        return Vector2.zero;
    }

    private void Set_ToolBox()
    {
        if (coroutine != null) return;

        if (_droppedToolBox != null) return;

        Vehicle_Controller vehicle = npcController.mainController.currentVehicle;
        if (vehicle.movement.onBoard) return;

        Set_Coroutine(StartCoroutine(Set_ToolBox_Coroutine()));
    }
    private IEnumerator Set_ToolBox_Coroutine()
    {
        NPC_Movement movement = npcController.movement;

        movement.Stop_FreeRoam();
        movement.Assign_TargetPosition(ToolBox_SetPosition());

        while (movement.At_TargetPosition() == false) yield return null;

        Drop_ToolBox();

        movement.Free_Roam(0);

        Set_Coroutine(null);
        yield break;
    }


    private bool ToolBox_NearbyVehicle()
    {
        Vehicle_Controller vehicle = npcController.mainController.currentVehicle;
        List<Vector2> allPositions = vehicle.positionClaimer.All_Positions();

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
    private void Upgrade_ExportRange()
    {
        // check nugget amount

        Vehicle_Controller vehicle = npcController.mainController.currentVehicle;
        int currentIndexNum = vehicle.InteractAreaSize_IndexNum(vehicle.interactArea.size);

        vehicle.Update_InteractArea(currentIndexNum + 1);

        Debug.Log("Upgrade_ExportRange");
    }

    private void Upgrade_MoveSpeed()
    {
        // check nugget amount

        Debug.Log("Upgrade_MoveSpeed");
    }
}