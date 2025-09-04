using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Controller : MonoBehaviour, ISaveLoadable
{
    [Space(20)]
    [SerializeField] private PrefabSpawner _npcSpawner;
    [SerializeField] private GameObject[] _allCraftNPC;

    [Space(10)] 
    [SerializeField] [Range(0, 100)] private float _defaultSpawnRate;
    [SerializeField] [Range(0, 100)] private float _spawnIncreaseRate;
    

    private CraftNPC_ControllerData _data;
    public CraftNPC_ControllerData data => _data;

    private Coroutine _cycleCoroutine;


    // MonoBehaviour
    private void Awake()
    {
        Load_Data();
    }

    private void Start()
    {
        Toggle_NPCSprites(false);
        Spawn();

        // subscription
        Main_Controller.instance.worldMap.OnNewLocation += _data.Reset_SpawnStatus;
        GlobalTime_Controller.instance.OnDayTime += Cycle_New;
    }

    private void OnDestroy()
    {
        // subscription
        Main_Controller.instance.worldMap.OnNewLocation += _data.Reset_SpawnStatus;
        GlobalTime_Controller.instance.OnDayTime -= Cycle_New;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        if (_data == null) return;
        if (_cycleCoroutine != null) Spawn_New(true);
        
        ES3.Save("CraftNPC_Controller/CraftNPC_ControllerData", _data);
        
        if (_data.currentNPC == null) return;
        _data.currentNPC.Invoke_OnSave();
    }

    public void Load_Data()
    {
        CraftNPC_ControllerData newData = new CraftNPC_ControllerData(NewNPC_IndexNum(), _defaultSpawnRate);
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
        
        int newIndex = Random.Range(0, _allCraftNPC.Length);
        if (_data == null) return newIndex;

        int currentIndex = _data.npcIndex;
        if (newIndex != currentIndex) return newIndex;

        return (newIndex + 1) % _allCraftNPC.Length;;
    }


    // Spawn
    private Vector3 Default_SpawnPosition()
    {
        Vector2 spawnPosition = Main_Controller.instance.currentLocation.OuterLocation_Position(-1);
        return spawnPosition;
    }


    private CraftNPC Spawn(int indexNum)
    {
        _data = new(indexNum, _defaultSpawnRate);
        
        GameObject getNPC = _allCraftNPC[indexNum];
        _npcSpawner.Set_Prefab(getNPC);

        GameObject spawnNPC = _npcSpawner.Spawn_Prefab(Default_SpawnPosition());
        spawnNPC.transform.SetParent(Main_Controller.instance.characterFile);

        CraftNPC craftNPC = spawnNPC.GetComponent<CraftNPC>();
        _data.Set_CurrentNPC(craftNPC);
        
        return craftNPC;
    }
    private void Spawn()
    {
        if (_data.isSpawned == false) return;
        if (_data.currentNPC != null) return;

        Spawn(_data.npcIndex).transform.position = Default_SpawnPosition();
    }

    private void Spawn_New()
    {
        // save current npc data
        if (_data.currentNPC != null) _data.currentNPC.Invoke_OnSave();
        
        // set new npc & default position
        Spawn(NewNPC_IndexNum()).transform.position = Default_SpawnPosition();
    }
    public void Spawn_New(bool spawnRateEffected)
    {
        if (spawnRateEffected && Utility.Percentage_Activated(_data.spawnRate) == false)
        {
            _data.Update_SpawnRate(_spawnIncreaseRate);
            _data.Reset_SpawnStatus();

            return;
        }
        
        Spawn_New();
    }

    private void Cycle_New()
    {
        if (_data.currentNPC == null)
        {
            Spawn_New(true);
            return;
        }
        
        if (_data.currentNPC.purchaseData.purchased) return;
        
        _cycleCoroutine = StartCoroutine(Cycle_New_Coroutine());
    }
    private IEnumerator Cycle_New_Coroutine()
    {
        CraftNPC currentNPC = _data.currentNPC;
        NPC_Controller controller = currentNPC.npcController;
        NPC_Movement movement = controller.movement;
        
        currentNPC.Invoke_OnSave();
        controller.interactable.LockInteract(true);
        
        movement.Set_MoveSpeed(movement.defaultMoveSpeed + 3);
        movement.Leave(0);
        
        while (movement.At_TargetPosition() == false) yield return null;
        
        Spawn_New(true);
        
        _cycleCoroutine = null;
        yield break;
    }
}