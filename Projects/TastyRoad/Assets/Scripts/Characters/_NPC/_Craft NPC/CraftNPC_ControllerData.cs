using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_ControllerData
{
    private CraftNPC _currentNPC;
    public CraftNPC currentNPC => _currentNPC;

    [ES3NonSerializable] private int _npcIndex;
    public int npcIndex => _npcIndex;


    // CraftNPC_ControllerData
    public CraftNPC_ControllerData(int npcIndex)
    {
        _npcIndex = npcIndex;
    }


    //
    public void Set_CurrentNPC(CraftNPC setNPC)
    {
        _currentNPC = setNPC;
    }
}