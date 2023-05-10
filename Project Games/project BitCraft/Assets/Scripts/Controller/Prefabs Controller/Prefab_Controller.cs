using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefab_Controller : MonoBehaviour
{
    private TileMap_Controller _tilemapController;
    public TileMap_Controller tilemapController { get => _tilemapController; set => _tilemapController = value; }

    private Prefab_Tag _prefabTag;
    public Prefab_Tag prefabTag { get => _prefabTag; set => _prefabTag = value; }

    [Header("Map Position")]
    [SerializeField] private int _positionX;
    public int positionX { get => _positionX; set => _positionX = value; }
    [SerializeField] private int _positionY;
    public int positionY { get => _positionY; set => _positionY = value; }

    [Header("Tile Position")]
    [SerializeField] private int _currentRowNum;
    public int currentRowNum { get => _currentRowNum; set => _currentRowNum = value; }
    [SerializeField] private int _currentColumnNum;
    public int currentColumnNum { get => _currentColumnNum; set => _currentColumnNum = value; }

    [Header("Interaction Data")]
    [SerializeField] private float _moveSpeed;
    public float moveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

    public void Connect_Components(TileMap_Controller tilemapController)
    {
        this.tilemapController = tilemapController;
        if (gameObject.TryGetComponent(out Prefab_Tag prefabTag)) { this.prefabTag = prefabTag; }
    }

    public void Update_Map_Position(int positionX, int positionY)
    {
        this.positionX = positionX;
        this.positionY = positionY;
    }
    public void Update_Tile_Position(int rowNum, int columnNum)
    {
        currentRowNum = rowNum;
        currentColumnNum = columnNum;
    }
}