using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Mechanic : CraftNPC
{
    [SerializeField] private ActionSelector _toolBox;
    [SerializeField] private Sprite[] _actionIcons;


    // MonoBehaviour
    private new void Start()
    {
        base.Start();

        AmountBar timeBar = controller.timeBar;

        timeBar.Set_Amount(controller.controller.foodIcon.AllDatas().Count);
        timeBar.Load();

        controller.nuggetBar.Load();

        _toolBox.transform.localPosition = Vector2.zero;
        _toolBox.gameObject.SetActive(false);
    }


    // Current Instance
    public void Set_Instance()
    {
        if (controller.currentCraftNPC != this) return;

        // load data
        AmountBar nuggetBar = controller.nuggetBar;
        nuggetBar.Set_Amount(ES3.Load("CraftNPC_Mechanic/nuggetAmount", nuggetBar.currentAmount));

        FoodData_Controller foodIcon = controller.controller.foodIcon;
        foodIcon.Update_AllDatas(ES3.Load("CraftNPC_Mechanic/foodData", foodIcon.AllDatas()));

        // subscriptions
        controller.controller.interactable.OnHoldIInteract += Drop_ToolBox;
    }

    public void Save_Instacne()
    {
        if (controller.currentCraftNPC != this) return;

        // save data
        ES3.Save("CraftNPC_Mechanic/nuggetAmount", controller.nuggetBar.currentAmount);
        ES3.Save("CraftNPC_Mechanic/foodData", controller.controller.foodIcon.AllDatas());

        // subscriptions
        controller.controller.interactable.OnHoldIInteract -= Drop_ToolBox;
    }


    //
    private void Upgrade_ExportRange()
    {
        Debug.Log("Upgrade_ExportRange");
    }

    private void Upgrade_MoveSpeed()
    {
        Debug.Log("Upgrade_MoveSpeed");
    }


    private void Drop_ToolBox()
    {
        if (_toolBox.gameObject.activeSelf) return;

        Main_Controller main = controller.controller.mainController;

        if (_toolBox.positionClaimer.CurrentPositions_Claimed()) return;

        _toolBox.gameObject.SetActive(true);

        // drop on current snap point
        Vector2 dropPosition = _toolBox.positionClaimer.Claim_CurrentPositions();

        _toolBox.transform.position = dropPosition;
        _toolBox.transform.SetParent(main.otherFile);

        // subscriptions
        _toolBox.Subscribe_Action(Upgrade_ExportRange);
        _toolBox.Subscribe_Action(Upgrade_MoveSpeed);
    }

    private void Collect_ToolBox()
    {
        if (_toolBox.gameObject.activeSelf == false) return;

        _toolBox.gameObject.SetActive(false);

        // collect
        _toolBox.positionClaimer.UnClaim_CurrentPositions();

        _toolBox.transform.SetParent(transform);
        _toolBox.transform.position = Vector2.zero;

        // reset subscriptions
        _toolBox.Reset_Subscriptions();
    }
}