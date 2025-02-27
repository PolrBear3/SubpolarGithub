using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Smith : CraftNPC
{
    // MonoBehaviour
    private void Awake()
    {
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
        ES3.Save("CraftNPC_Smith/nuggetBar.currentAmount", nuggetBar.currentAmount);
        ES3.Save("CraftNPC_Smith/npcController.foodIcon.AllDatas()", main.foodIcon.AllDatas());
    }

    private void Load_Data()
    {
        nuggetBar.Set_Amount(ES3.Load("CraftNPC_Smith/nuggetBar.currentAmount", nuggetBar.currentAmount));
        main.foodIcon.Update_AllDatas(ES3.Load("CraftNPC_Smith/npcController.foodIcon.AllDatas()", main.foodIcon.AllDatas()));
    }
}
