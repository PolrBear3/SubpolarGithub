using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Smith : CraftNPC
{
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
}
