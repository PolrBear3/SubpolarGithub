using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;
    public Main_Controller mainController => _mainController;


    [Header("")]
    [SerializeField] private GameObject _spawnPrefab;

    [Header("")]
    [SerializeField] private bool _randomAmountSpawn;
    [Range(0, 10)][SerializeField] private int _spawnAmount;

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

    public void Hold_Interact()
    {

    }

    public void UnInteract()
    {

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
            spawnPrefab.transform.SetParent(_mainController.otherFile);
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
            int randAmount = Random.Range(1, _spawnAmount);
            return randAmount;
        }

        return _spawnAmount;
    }


    private void Random_Spawn()
    {
        Location_Controller location = _mainController.currentLocation;

        for (int i = 0; i < Spawn_Amount(); i++)
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
