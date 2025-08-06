using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftNPC_ControllerData
{
    [ES3NonSerializable] private CraftNPC _currentNPC;
    public CraftNPC currentNPC => _currentNPC;

    [ES3Serializable] private int _npcIndex;
    public int npcIndex => _npcIndex;

    [ES3Serializable] private float _spawnRate;
    public float spawnRate => _spawnRate;
    
    [ES3Serializable] private bool _isSpawned;
    public bool isSpawned => _isSpawned;



    // CraftNPC_ControllerData
    public CraftNPC_ControllerData(int npcIndex, float defaultSpawnRate)
    {
        _npcIndex = npcIndex;
        _spawnRate = defaultSpawnRate;
    }


    // Data Control
    public void Set_CurrentNPC(CraftNPC setNPC)
    {
        _currentNPC = setNPC;

        _isSpawned = true;
    }

    public void Update_SpawnRate(float updateValue)
    {
        _spawnRate += updateValue;
        _spawnRate = Mathf.Clamp(_spawnRate, 0f, 100f);
        
        _isSpawned = true;
    }

    public void Reset_SpawnStatus()
    {
        _isSpawned = false;
    }
}