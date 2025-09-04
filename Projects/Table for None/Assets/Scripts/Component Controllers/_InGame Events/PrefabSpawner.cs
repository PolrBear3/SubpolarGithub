using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private GameObject _spawnPrefab;

    [Space(20)]
    [Range(0, 100)][SerializeField] private int _spawnAmount;
    [Range(0, 100)][SerializeField] private int _minimumSpawnAmount;

    [Space(20)] 
    [SerializeField] private bool _restrictCrossSpawn;
    [SerializeField] private bool _restrictSurroundingSpawn;
    [SerializeField] private Vector2[] _spawnPositions;


    // Main Activation
    public void Spawn()
    {
        if (_spawnPositions.Length <= 0)
        {
            Random_Spawn();
            return;
        }

        Assign_Spawn();
    }


    // Functions
    public void Set_Prefab(GameObject setPrefab)
    {
        _spawnPrefab = setPrefab;
    }

    public GameObject Spawn_Prefab(Vector2 spawnPosition)
    {
        if (_spawnPrefab == null) return null;
        GameObject spawnPrefab = Instantiate(_spawnPrefab, spawnPosition, Quaternion.identity);
        
        Main_Controller main = Main_Controller.instance;
        
        // check if spawn prefab is a station
        if (spawnPrefab.TryGetComponent(out Station_Controller station) == false)
        {
            main.data.Claim_Position(spawnPrefab.transform.position);
            
            // set as a child of current location file
            spawnPrefab.transform.SetParent(main.otherFile);
            return spawnPrefab;
        }

        // set station
        station.Set_Data(new(station.stationScrObj, station.transform.position));
        
        if (station.movement !=null) station.movement.Load_Position();
        else main.data.Claim_Position(station.transform.position);
        
        return spawnPrefab;
    }


    private int Spawn_Amount()
    {
        if (_minimumSpawnAmount >= _spawnAmount) return _spawnAmount;
        
        int randAmount = Random.Range(_minimumSpawnAmount, _spawnAmount + 1);
        return randAmount;
    }


    private bool SpawnPosition_Restricted(List<Vector2> spawnedPositions, Vector2 centerPosition)
    {
        if (!_restrictCrossSpawn && !_restrictSurroundingSpawn) return false;
        
        List<Vector2> restrictedPositions = _restrictSurroundingSpawn ? Utility.Surrounding_SnapPositions(centerPosition)
                : Utility.CrossSurrounding_SnapPositions(centerPosition);

        for (int i = 0; i < restrictedPositions.Count; i++)
        {
            if (spawnedPositions.Contains(restrictedPositions[i]) == false) continue;
            return true;
        }

        return false;
    }
    
    private void Random_Spawn()
    {
        Main_Controller main = Main_Controller.instance;
        Location_Controller location = main.currentLocation;

        List<Vector2> spawnPositions = location.All_SpawnPositions();
        List<Vector2> spawnedPositions = new();

        for (int i = 0; i < Spawn_Amount(); i++)
        {
            while (spawnPositions.Count > 0)
            {
                int randIndex = Random.Range(0, spawnPositions.Count);
                Vector2 randPos = spawnPositions[randIndex];
                
                spawnPositions.RemoveAt(randIndex);
                
                if (main.data.Position_Claimed(randPos)) continue;
                if (SpawnPosition_Restricted(spawnedPositions, randPos)) continue;

                spawnedPositions.Add(randPos);
                Spawn_Prefab(randPos);
                
                break;
            }
        }
    }

    
    private void Assign_Spawn()
    {
        Location_Controller location = Main_Controller.instance.currentLocation;

        int spawnCount = Spawn_Amount();

        for (int i = 0; i < _spawnPositions.Length; i++)
        {
            if (spawnCount <= 0) return;
            if (location.Is_OuterSpawnPoint(_spawnPositions[i])) continue;

            Spawn_Prefab(_spawnPositions[i]);
            spawnCount--;
        }
    }
}
