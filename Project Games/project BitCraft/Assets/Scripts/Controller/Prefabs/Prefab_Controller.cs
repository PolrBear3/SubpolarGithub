using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefab_Controller : MonoBehaviour
{
    private SpriteRenderer _sr;
    
    private TileMap_Controller _tilemapController;
    public TileMap_Controller tilemapController { get => _tilemapController; set => _tilemapController = value; }

    private Prefab_Tag _prefabTag;
    public Prefab_Tag prefabTag { get => _prefabTag; set => _prefabTag = value; }

    private Health_Controller _healthController;
    public Health_Controller healthController { get => _healthController; set => _healthController = value; }

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
    [SerializeField] private Vector2 _setPosition;
    public Vector2 setPosition { get => _setPosition; set => _setPosition = value; }
    [SerializeField] private float _moveSpeed;
    public float moveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

    [SerializeField] private List<Prefab_Tag> _dropAvailableTiles = new List<Prefab_Tag>();

    [SerializeField] private int _currentAmount;
    public int currentAmount { get => _currentAmount; set => _currentAmount = value; }

    [Header("Sprite Control Data")]
    [SerializeField] private int _changeAmount;
    [SerializeField] private List<Sprite> _sprites = new List<Sprite>();

    // Check
    public bool Is_Prefab_Type(Prefab_Type type)
    {
        if (prefabTag.prefabType == type) return true;
        return false;
    }

    public bool Drop_Available(int tilePrefabID)
    {
        // all tiles available
        if (_dropAvailableTiles.Count <= 0) return true;

        for (int i = 0; i < _dropAvailableTiles.Count; i++)
        {
            if (_dropAvailableTiles[i].prefabID == tilePrefabID) return true;
        }
        return false;
    }

    // Connection
    public void Connect_Components(TileMap_Controller tilemapController)
    {
        this.tilemapController = tilemapController;

        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
        if (gameObject.TryGetComponent(out Prefab_Tag prefabTag)) { this.prefabTag = prefabTag; }
        if (gameObject.TryGetComponent(out Health_Controller healthController)) { this.healthController = healthController; }
    }

    // Updates
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

    private void Sprite_Update()
    {
        if (_sprites.Count <= 0) return;
        int spriteNum = (_currentAmount / _changeAmount) - 1;
        
        if (spriteNum <= 0) spriteNum = 0;
        if (spriteNum >= _sprites.Count - 1) spriteNum = _sprites.Count - 1;
        
        if (_sr.sprite == _sprites[spriteNum]) return;
        _sr.sprite = _sprites[spriteNum];
    }
    public void Sprite_LayerOrder_Update()
    {
        _sr.sortingOrder = (_currentColumnNum * 10) + _prefabTag.layerOrderNum;
    }

    // Amount Control
    public void Increase_Amount(int amount)
    {
        _currentAmount += amount;
        Sprite_Update();
    }
    public void Decrease_Amount(int amount)
    {
        _currentAmount -= amount;
        Sprite_Update();
    }
}