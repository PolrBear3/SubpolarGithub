using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;

    [Header("")]
    [SerializeField] private GameObject _spawnPrefab;
    [Range(0, 10)] [SerializeField] private int _spawnAmount;

    [Header("")]
    [SerializeField] private Vector2[] _spawnPositions;


    // UnityEngine
    private void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }


    // IInteractable
    public void Interact()
    {
        if (_spawnPositions.Length <= 0)
        {
            Random_Spawn();
            return;
        }

        Assign_Spawn();
    }

    public void UnInteract()
    {
        
    }


    // Functions
    private void Spawn_Prefab(Vector2 spawnPosition)
    {
        GameObject spawnPrefab = Instantiate(_spawnPrefab, spawnPosition, Quaternion.identity);

        // check if spawn prefab is a station
        if (spawnPrefab.TryGetComponent(out Station_Movement movement) == false)
        {
            // set as a child of current location file
            spawnPrefab.transform.SetParent(_mainController.otherFile);
            return;
        }

        // set station
        movement.Load_Position();
    }


    private void Random_Spawn()
    {
        Location_Controller location = _mainController.currentLocation;

        for (int i = 0; i < _spawnAmount; i++)
        {
            // find available position
            Vector2 setPosition = location.Random_SnapPosition();

            while (location.Restricted_Position(setPosition))
            {
                setPosition = location.Random_SnapPosition();
            }

            Spawn_Prefab(setPosition);
        }
    }

    private void Assign_Spawn()
    {
        Location_Controller location = _mainController.currentLocation;

        int spawnCount = _spawnAmount;

        for (int i = 0; i < _spawnPositions.Length; i++)
        {
            if (spawnCount <= 0) return;
            if (location.Restricted_Position(_spawnPositions[i])) continue;

            Spawn_Prefab(_spawnPositions[i]);
            spawnCount--;
        }
    }
}
