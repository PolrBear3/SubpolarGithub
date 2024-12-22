using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Mechanic : CraftNPC
{
    [Header("")]
    [SerializeField] private ActionSelector _toolBox;


    // MonoBehaviour
    private void Awake()
    {
        Subscribe_OnSave(Save_Data);
        Load_Data();
    }


    // Private Save and Load
    private void Load_Data()
    {
        nuggetBar.Set_Amount(ES3.Load("CraftNPC_Mechanic/nuggetBar.currentAmount", nuggetBar.currentAmount));
        giftBar.Set_Amount(ES3.Load("CraftNPC_Mechanic/giftBar.currentAmount", giftBar.currentAmount));
    }

    private void Save_Data()
    {
        ES3.Save("CraftNPC_Mechanic/nuggetBar.currentAmount", nuggetBar.currentAmount);
        ES3.Save("CraftNPC_Mechanic/giftBar.currentAmount", giftBar.currentAmount);
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

        Main_Controller main = npcController.mainController;

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