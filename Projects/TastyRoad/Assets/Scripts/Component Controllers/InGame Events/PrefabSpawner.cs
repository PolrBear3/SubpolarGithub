using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour, IInteractable
{
    private Main_Controller _mainController;

    [SerializeField] private GameObject _spawnPrefab;


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

    }
}
