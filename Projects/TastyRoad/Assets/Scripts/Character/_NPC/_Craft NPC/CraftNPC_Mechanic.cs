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

        // subscriptions
        ActionBubble_Interactable interactable = npcController.interactable;

        interactable.OnIInteract += Drop_ToolBox;
        interactable.OnIInteract += Update_ActionBubble;

        interactable.OnAction1Input += Purchase;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        // subscriptions
        ActionBubble_Interactable interactable = npcController.interactable;

        interactable.OnIInteract -= Drop_ToolBox;
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

    private void Collect_ToolBox()
    {
        if (_droppedToolBox == null) return;

        GameObject currentToolBox = _droppedToolBox.gameObject;

        _droppedToolBox = null;
        Destroy(currentToolBox);
    }

    private void Purchase()
    {
        if (_droppedToolBox == null) return;

        _droppedToolBox.Invoke_Action();
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