using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;

    [SerializeField] private GameObject _spawnPrefab;
    [Range(0, 10)] [SerializeField] private int _spawnAmount;


    // UnityEngine
    private void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }


    // IInteractable
    public void Interact()
    {
        Spawn();
    }

    public void UnInteract()
    {
        
    }


    // Functions
    private void Spawn()
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

            // spawn on empty random position
            GameObject spawnPrefab = Instantiate(_spawnPrefab, setPosition, Quaternion.identity);

            // check if spawn prefab is a station
            if (spawnPrefab.TryGetComponent(out Station_Movement movement) == false)
            {
                // set as a child of current location file
                spawnPrefab.transform.SetParent(_mainController.otherFile);
                continue;
            }

            // set station
            movement.Load_Position();
        }
    }
}
