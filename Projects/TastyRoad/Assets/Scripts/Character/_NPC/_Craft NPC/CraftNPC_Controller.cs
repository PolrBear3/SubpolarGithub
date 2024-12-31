using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Controller : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private PrefabSpawner _npcSpawner;

    [Header("")]
    [SerializeField] private GameObject[] _allCraftNPC;


    private CraftNPC _currentNPC;
    private int _npcIndexNum;


    // MonoBehaviour
    private void Start()
    {
        Toggle_NPCSprites(false);
        Spawn(_npcIndexNum).transform.position = Default_SpawnPosition();

        // subscription
        WorldMap_Controller.NewLocation_Event += Spawn_New;
    }

    private void OnDestroy()
    {
        // subscription
        WorldMap_Controller.NewLocation_Event -= Spawn_New;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        _currentNPC.Invoke_OnSave();

        ES3.Save("CraftNPC_Controller/_npcIndexNum", _npcIndexNum);
    }

    public void Load_Data()
    {
        LoadNPC_IndexNum();
    }


    // All
    private void Toggle_NPCSprites(bool toggle)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(toggle);
        }
    }


    // Data
    private int LoadNew_IndexNum()
    {
        // remove previous index num !

        _npcIndexNum = Random.Range(0, _allCraftNPC.Length);
        return _npcIndexNum;
    }

    private void LoadNPC_IndexNum()
    {
        if (ES3.KeyExists("CraftNPC_Controller/_npcIndexNum") == false)
        {
            LoadNew_IndexNum();
            return;
        }

        int loadIndexNum = ES3.Load("CraftNPC_Controller/_npcIndexNum", _npcIndexNum);
        _npcIndexNum = Mathf.Clamp(loadIndexNum, 0, _allCraftNPC.Length - 1);
    }


    // Spawn
    private Vector3 Default_SpawnPosition()
    {
        Vector2 spawnPosition = _npcSpawner.mainController.currentLocation.OuterLocation_Position(-1);
        return spawnPosition;
    }


    private CraftNPC Spawn(int indexNum)
    {
        GameObject getNPC = _allCraftNPC[indexNum];

        _npcSpawner.Set_Prefab(getNPC);

        GameObject spawnNPC = _npcSpawner.Spawn_Prefab(Default_SpawnPosition());
        spawnNPC.transform.SetParent(transform);

        CraftNPC craftNPC = spawnNPC.GetComponent<CraftNPC>();

        _currentNPC = craftNPC;
        _npcIndexNum = indexNum;

        return craftNPC;
    }

    private void Spawn_New()
    {
        // save current npc data
        _currentNPC.Invoke_OnSave();

        // reset
        GameObject destroyNPC = _currentNPC.gameObject;

        _currentNPC = null;
        Destroy(destroyNPC);

        // set new npc & default position
        Spawn(LoadNew_IndexNum()).transform.position = Default_SpawnPosition();
    }
}