using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _spawnPrefab;

    [Header("")]
    [SerializeField] private bool _randomAmountSpawn;

    [Header("")]
    [Range(0, 10)][SerializeField] private int _spawnAmount;
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

        // check if spawn prefab is a station
        if (spawnPrefab.TryGetComponent(out Station_Movement movement) == false)
        {
            // set as a child of current location file
            spawnPrefab.transform.SetParent(Main_Controller.instance.otherFile);
            return spawnPrefab;
        }

        // set station
        movement.Load_Position();
        return spawnPrefab;
    }


    private int Spawn_Amount()
    {
        if (_randomAmountSpawn)
        {
            int randAmount = Random.Range(1, _spawnAmount + 1);
            return randAmount;
        }

        return _spawnAmount;
    }


    private void Random_Spawn()
    {
        Location_Controller location = Main_Controller.instance.currentLocation;

        for (int i = 0; i < Spawn_Amount(); i++)
        {
            // find available position
            Vector2 setPosition = location.Random_SpawnPosition();

            while (location.Restricted_Position(setPosition))
            {
                setPosition = location.Random_SpawnPosition();
            }

            Spawn_Prefab(setPosition);
        }
    }

    private void Assign_Spawn()
    {
        Location_Controller location = Main_Controller.instance.currentLocation;

        int spawnCount = Spawn_Amount();

        for (int i = 0; i < _spawnPositions.Length; i++)
        {
            if (spawnCount <= 0) return;
            if (location.Restricted_Position(_spawnPositions[i])) continue;

            Spawn_Prefab(_spawnPositions[i]);
            spawnCount--;
        }
    }
}
