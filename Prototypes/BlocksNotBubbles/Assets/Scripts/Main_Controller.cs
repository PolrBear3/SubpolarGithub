using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GridManager _gridManager;
    public GridManager gridManager => _gridManager;

    [SerializeField] private Launcher _launcher;
    public Launcher launcher => _launcher;


    [Header("")]
    [SerializeField] private GameObject _blockObject;
    public GameObject blockObject => _blockObject;


    [Header("")]
    [SerializeField] private Vector2[] _surroundDirections;
    public Vector2[] surroundDirections => _surroundDirections;

    [SerializeField] private Block_ScrObj[] _blockTypes;
    public Block_ScrObj[] blockTypes => _blockTypes;


    public static Main_Controller instance;


    // MonoBehaviour
    private void Awake()
    {
        instance = this;
    }
}
