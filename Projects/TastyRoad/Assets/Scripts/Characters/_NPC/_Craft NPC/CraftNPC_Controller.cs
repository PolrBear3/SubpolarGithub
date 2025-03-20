using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Controller : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private PrefabSpawner _npcSpawner;

    [Header("")]
    [SerializeField] private GameObject[] _allCraftNPC;


    private CraftNPC_ControllerData _data;
    public CraftNPC_ControllerData data => _data;


    // MonoBehaviour
    private void Awake()
    {
        Load_Data();
    }

    private void Start()
    {
        transform.SetParent(Main_Controller.instance.characterFile);

        Toggle_NPCSprites(false);
        Spawn(_data.npcIndex).transform.position = Default_SpawnPosition();

        // subscription
        WorldMap_Controller.OnNewLocation += Spawn_New;
    }

    private void OnDestroy()
    {
        // subscription
        WorldMap_Controller.OnNewLocation -= Spawn_New;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        if (_data == null) return;

        _data.currentNPC.Invoke_OnSave();

        ES3.Save("CraftNPC_Controller/CraftNPC_ControllerData", _data);
    }

    public void Load_Data()
    {
        CraftNPC_ControllerData newData = new CraftNPC_ControllerData(Random.Range(0, _allCraftNPC.Length));
        CraftNPC_ControllerData loadData = ES3.Load("CraftNPC_Controller/CraftNPC_ControllerData", newData);

        _data = loadData;
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
    private int NewNPC_IndexNum()
    {
        if (_allCraftNPC.Length <= 1) return 0;

        int currentIndex = _data.npcIndex;
        int newIndex = currentIndex;

        while (newIndex == currentIndex)
        {
            newIndex = Random.Range(0, _allCraftNPC.Length);
        }
        return newIndex;
    }


    // Spawn
    private Vector3 Default_SpawnPosition()
    {
        Vector2 spawnPosition = Main_Controller.instance.currentLocation.OuterLocation_Position(-1);
        return spawnPosition;
    }


    private CraftNPC Spawn(int indexNum)
    {
        _data = new(indexNum);

        GameObject getNPC = _allCraftNPC[indexNum];
        _npcSpawner.Set_Prefab(getNPC);

        GameObject spawnNPC = _npcSpawner.Spawn_Prefab(Default_SpawnPosition());
        spawnNPC.transform.SetParent(transform);

        CraftNPC craftNPC = spawnNPC.GetComponent<CraftNPC>();
        _data.Set_CurrentNPC(craftNPC);

        return craftNPC;
    }

    private void Spawn_New()
    {
        // save current npc data
        _data.currentNPC.Invoke_OnSave();

        // set new npc & default position
        Spawn(NewNPC_IndexNum()).transform.position = Default_SpawnPosition();
    }
}