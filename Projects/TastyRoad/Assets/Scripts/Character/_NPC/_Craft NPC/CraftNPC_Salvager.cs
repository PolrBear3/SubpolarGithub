using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Salvager : CraftNPC
{
    // MonoBehaviour
    private void Awake()
    {
        Subscribe_OnSave(Save_Data);
        Load_Data();
    }


    // private Save and Load
    private void Load_Data()
    {
        nuggetBar.Set_Amount(ES3.Load("CraftNPC_Salvager/nuggetBar.currentAmount", nuggetBar.currentAmount));
        giftBar.Set_Amount(ES3.Load("CraftNPC_Salvager/giftBar.currentAmount", giftBar.currentAmount));
    }

    private void Save_Data()
    {
        ES3.Save("CraftNPC_Salvager/nuggetBar.currentAmount", nuggetBar.currentAmount);
        ES3.Save("CraftNPC_Salvager/giftBar.currentAmount", giftBar.currentAmount);
    }
}
