using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buddy_Controller : MonoBehaviour, ISaveLoadable
{
    [Space(20)]
    [SerializeField] private GameObject _buddyNPC;
    public GameObject buddyNPC => _buddyNPC;
        
    [Space(20)]
    [SerializeField] [Range(0, 10)] private int _defaultBuddyCount;
    
    [SerializeField] [Range(0, 6)] private int _defaultMergeCount;
    public int defaultMergeCount => _defaultMergeCount;
    
    [Space(60)] 
    [SerializeField] private Ability_ScrObj _multipleBuddiesAbility;

    
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
        List<BuddyNPC_Data> buddyDatas = new();
        
        foreach (Buddy_NPC buddy in _currentBuddies)
        {
            buddyDatas.Add(buddy.data);
        }
        
        ES3.Save("Buddy_Controller/BuddyNPC_Datas", buddyDatas);
    }

    public void Load_Data()
    {
        
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
        List<BuddyNPC_Data> loadBuddyDatas = ES3.Load("Buddy_Controller/BuddyNPC_Datas", new List<BuddyNPC_Data>());

        foreach (BuddyNPC_Data loadData in loadBuddyDatas)
        {
            Vector2 spawnPosition = Main_Controller.instance.currentVehicle.Random_InteractPoint();
            GameObject loadBuddy = Instantiate(_buddyNPC, spawnPosition, Quaternion.identity);
            
            Buddy_NPC buddy = loadBuddy.GetComponent<Buddy_NPC>();
            Track_CurrentBuddy(buddy);

            buddy.Set_Data(loadData);
            buddy.Load_DataIndication();
        }
    }

    public void Track_CurrentBuddy(Buddy_NPC trackBuddy)
    {
        _currentBuddies.Add(trackBuddy);
        
        trackBuddy.controller.interactable.LockInteract(true);
        
        Main_Controller main = Main_Controller.instance;
        trackBuddy.transform.SetParent(main.characterFile);
        
        int activationCount = main.Player().abilityManager.data.Ability_ActivationCount(_multipleBuddiesAbility);
        int maxBuddycount = _maxBuddyCount + activationCount * _multipleBuddiesAbility.ActivationStep_Value();
        
        if (_currentBuddies.Count <= maxBuddycount) return;
        Remove_Buddy(_currentBuddies[0]);
    }

    public void Remove_Buddy(Buddy_NPC buddy)
    {
        // spawn normal npc
        GameObject spawnNPC = Main_Controller.instance.Spawn_Character(1, buddy.transform.position);
        NPC_Controller normalNPC = spawnNPC.GetComponent<NPC_Controller>();

        Main_Controller main = Main_Controller.instance;
        LocationData data = main.currentLocation.data;
        
        normalNPC.basicAnim.Set_OverrideController(data.Random_NPCSkinOverride());
        normalNPC.movement.Free_Roam(data.roamArea, normalNPC.movement.Random_IntervalTime());
        
        // remove old buddy
        _currentBuddies.Remove(buddy);
        
        main.UnTrack_CurrentCharacter(buddy.gameObject);
        Destroy(buddy.gameObject);
    }
}
