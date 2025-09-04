using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Clickable_EventSystem _eventSystem;

    private Tile_Data _data;
    public Tile_Data data => _data;


    // MonoBehaviour
    private void Start()
    {
        _eventSystem.OnClick += Spawn_Switch;
    }

    private void OnDestroy()
    {
        _eventSystem.OnClick -= Spawn_Switch;
    }


    // Main
    private void Spawn_Switch()
    {
        Debug.Log("This is Tile");
    }
}
