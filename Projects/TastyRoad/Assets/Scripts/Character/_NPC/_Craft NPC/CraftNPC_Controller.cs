using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Controller : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private NPC_Controller _controller;
    public NPC_Controller controller => _controller;


    [Header("")]
    [SerializeField] private CraftNPC[] _allCraftNPC;

    private CraftNPC _currentCraftNPC;
    public CraftNPC currentCraftNPC => _currentCraftNPC;


    [Header("")]
    [SerializeField] private AmountBar _nuggetBar;
    public AmountBar nuggetBar => _nuggetBar;

    [SerializeField] private AmountBar _timeBar;
    public AmountBar timeBar => _timeBar;


    // MonoBehaviour
    private void Start()
    {
        Set_CraftNPC(_currentCraftNPC);

        // subscriptions
        WorldMap_Controller.NewLocation_Event += _currentCraftNPC.Invoke_OnSaveInstance;
        WorldMap_Controller.NewLocation_Event += Set_CraftNPC;
    }

    private void OnDestroy()
    {
        // subscriptions
        WorldMap_Controller.NewLocation_Event -= _currentCraftNPC.Invoke_OnSaveInstance;
        WorldMap_Controller.NewLocation_Event -= Set_CraftNPC;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("CraftNPC_Controller/CraftNPC_IndexNum", CraftNPC_IndexNum(_currentCraftNPC));
    }

    public void Load_Data()
    {
        _currentCraftNPC = _allCraftNPC[ES3.Load("CraftNPC_Controller/CraftNPC_IndexNum", 0)];
    }


    //
    private int CraftNPC_IndexNum(CraftNPC npc)
    {
        for (int i = 0; i < _allCraftNPC.Length; i++)
        {
            if (_allCraftNPC[i] != npc) continue;
            return i;
        }
        return 0;
    }


    private void Set_CraftNPC(CraftNPC setNPC)
    {
        if (setNPC == null)
        {
            Set_CraftNPC();
            return;
        }

        _currentCraftNPC = setNPC;
        _currentCraftNPC.SetInstance_CurrentNPC();
    }

    private void Set_CraftNPC()
    {
        int randIndex = Random.Range(0, _allCraftNPC.Length);
        CraftNPC setNPC = _allCraftNPC[randIndex];

        if (setNPC == null)
        {
            Debug.Log("Set_CraftNPC() random set null!");
            return;
        }

        Set_CraftNPC(_allCraftNPC[randIndex]);
    }
}