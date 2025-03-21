using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Smith : CraftNPC
{
    [Header("")]
    [SerializeField] private GameObject _smithTable;

    private ActionSelector _setTable;


    [Header("")]
    [SerializeField] private Station_ScrObj[] _modifyStations;


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
    }


    // private Save and Load
    private void Save_Data()
    {
        ES3.Save("CraftNPC_Smith/data", data);
    }

    private void Load_Data()
    {
        Set_Data(ES3.Load("CraftNPC_Smith/data", new CraftNPC_Data(false)));
    }


    // Set Table
    private bool Table_SetAvailable()
    {
        if (data.payed == false) return false;
        if (_setTable != null) return false;

        return false;
    }

    private Vector2 Table_SetPosition()
    {
        return Vector2.zero;
    }


    private void MoveTo_SetPosition()
    {
        if (Table_SetAvailable() == false) return;

        Set_Coroutine(StartCoroutine(MoveTo_SetPosition_Coroutine()));
    }
    private IEnumerator MoveTo_SetPosition_Coroutine()
    {
        Toggle_Coroutine(true);

        NPC_Movement movement = npcController.movement;
        movement.Stop_FreeRoam();

        while (movement.At_TargetPosition(Table_SetPosition()) == false) yield return null;

        Set_Table();

        Toggle_Coroutine(false);
        yield break;
    }

    private void Set_Table()
    {
        if (Table_SetAvailable() == false) return;
    }


    // Purchase Upgrades
}