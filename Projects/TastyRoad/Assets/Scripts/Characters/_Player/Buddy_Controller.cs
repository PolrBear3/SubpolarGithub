using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddyController_Data
{
    [ES3Serializable] private List<FoodData> _automateFoodDatas = new();
    public List<FoodData> automateFoodDatas => _automateFoodDatas;

    public BuddyController_Data(List<FoodData> saveFoodDatas)
    {
        _automateFoodDatas = saveFoodDatas;
    }
}

public class Buddy_Controller : MonoBehaviour, ISaveLoadable
{
    [Space(20)]
    [SerializeField] private GameObject _buddyNPC;
    public GameObject buddyNPC => _buddyNPC;
        
    [Space(20)]
    [SerializeField] [Range(0, 10)] private int _defaultBuddyCount;

    
    private BuddyController_Data _data;
    
    private int _maxBuddyCount;
    public int maxBuddyCount => _maxBuddyCount;
    
    private List<Buddy_NPC> _currentBuddies = new();
    public List<Buddy_NPC> currentBuddies => _currentBuddies;
    
    
    // MonoBehaviour
    private void Start()
    {
        _maxBuddyCount += _defaultBuddyCount;
        
        Spawn_SavedBuddies();
    }
    
    
    // ISaveLoadable
    public void Save_Data()
    {
        _data.automateFoodDatas.Clear();
        
        foreach (Buddy_NPC buddy in _currentBuddies)
        {
            _data.automateFoodDatas.Add(buddy.automateFoodData);
        }
        
        ES3.Save("Buddy_Controller/BuddyController_Data", _data);
    }

    public void Load_Data()
    {
        _data = ES3.Load("Buddy_Controller/BuddyController_Data", new BuddyController_Data(new()));
    }


    // Datas
    public Buddy_NPC Follow_Buddy(Buddy_NPC targetBuddy)
    {
        for (int i = _currentBuddies.Count - 1; i >= 0; i--)
        {
            if (i == 0) return null;
            if (targetBuddy != _currentBuddies[i]) continue;
            if (currentBuddies[i - 1].isFollowing == false) continue;
            
            return currentBuddies[i - 1];
        }

        return null;
    }


    // Buddies Control
    private void Spawn_SavedBuddies()
    {
        List<FoodData> loadedFoodDatas = _data.automateFoodDatas;

        foreach (FoodData loadData in loadedFoodDatas)
        {
            // set random spawn points to vehicle nearby positions //
            
            GameObject loadBuddy = Instantiate(_buddyNPC, transform.position, Quaternion.identity);
            Buddy_NPC buddy = loadBuddy.GetComponent<Buddy_NPC>();
        
            Track_CurrentBuddy(buddy, new(loadData));
        }
    }

    public void Track_CurrentBuddy(Buddy_NPC trackBuddy, FoodData automateFoodData)
    {
        _currentBuddies.Add(trackBuddy);
        
        trackBuddy.controller.interactable.LockInteract(true);
        trackBuddy.Set_AutomateFoodData(new(automateFoodData));
        
        Main_Controller main = Main_Controller.instance;
        trackBuddy.transform.SetParent(main.characterFile);

        if (_currentBuddies.Count <= _maxBuddyCount) return;
        
        Buddy_NPC oldBuddy = _currentBuddies[0];
        
        // spawn normal npc
        GameObject spawnNPC = Main_Controller.instance.Spawn_Character(1, oldBuddy.transform.position);
        NPC_Controller normalNPC = spawnNPC.GetComponent<NPC_Controller>();

        LocationData data = main.currentLocation.data;
        
        normalNPC.basicAnim.Set_OverrideController(data.Random_NPCSkinOverride());
        normalNPC.movement.Free_Roam(data.roamArea, normalNPC.movement.Random_IntervalTime());
        
        // remove old buddy
        _currentBuddies.Remove(oldBuddy);
        
        main.UnTrack_CurrentCharacter(oldBuddy.gameObject);
        Destroy(oldBuddy.gameObject);
    }
}
