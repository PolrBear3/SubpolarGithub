using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile_Controller : MonoBehaviour
{
    private SpriteRenderer _sr;

    [SerializeField] private List<Sprite> _sprites = new List<Sprite>(); 

    private TileMap_Controller _tilemapController;
    public TileMap_Controller tilemapController { get => _tilemapController; set => _tilemapController = value; }

    private Tile_Direction_System _directionSystem;
    public Tile_Direction_System directionSystem { get => _directionSystem; set => _directionSystem = value; }

    private Prefab_Tag _prefabTag;
    public Prefab_Tag prefabTag { get => _prefabTag; set => _prefabTag = value; }

    private int _rowNum;
    public int rowNum { get => _rowNum; set => _rowNum = value; }

    private int _columnNum;
    public int columnNum { get => _columnNum; set => _columnNum = value; }

    private bool _moveReady = false;
    public bool moveReady { get => _moveReady; set => _moveReady = value; }

    private bool _objectReady = false;
    public bool objectReady { get => _objectReady; set => _objectReady = value; }

    private bool _itemDropReady = false;
    public bool itemDropReady { get => _itemDropReady; set => _itemDropReady = value; }

    [Header("Current Prefabs Data")]
    [SerializeField] private Transform _prefabsParent;
    
    [SerializeField] private int _maxPrefabsAmount;

    [SerializeField] private List<GameObject> _currentPrefabs = new List<GameObject>();
    public List<GameObject> currentPrefabs { get => _currentPrefabs; set => _currentPrefabs = value; }

    [Header("Interaction Box")]
    [SerializeField] private GameObject objectInteractBox;
    [SerializeField] private GameObject itemDropkBox;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
        if (gameObject.TryGetComponent(out Prefab_Tag prefabTag)) { this.prefabTag = prefabTag; }
    }

    // Check
    public bool Found(int rowNum, int columnNum)
    {
        if (rowNum != this.rowNum) return false;
        if (columnNum != this.columnNum) return false;
        return true;
    }
    public bool Is_Prefab_Type(Prefab_Type type)
    {
        if (prefabTag.prefabType == type) return true;
        return false;
    }

    public bool Has_Prefab_Type(Prefab_Type type)
    {
        if (currentPrefabs.Count <= 0) return false;
        if (type == Prefab_Type.all) return true;

        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (currentPrefabs[i] == null) continue;
            if (!currentPrefabs[i].TryGetComponent(out Prefab_Tag tag)) continue;
            if (type != tag.prefabType) continue;
            return true;
        }

        return false;
    }
    public bool Has_Prefab_ID(Prefab_Type type, int searchID)
    {
        if (!Has_Prefab_Type(type)) return false;

        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (!currentPrefabs[i].TryGetComponent(out Prefab_Tag tag)) continue;
            if (searchID != tag.prefabID) continue;

            return true;
        }

        return false;
    }

    public bool Is_PrefabsAmount_Max()
    {
        if (currentPrefabs.Count <= 0) return false;

        int prefabsCount = 0;
        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (currentPrefabs == null) continue;
            prefabsCount++;
        }

        if (prefabsCount >= _maxPrefabsAmount) return true;
        else return false;
    }

    // Get
    public Prefab_Controller Get_Prefab_PrefabController(bool ignoreMax, int objectID)
    {
        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (currentPrefabs[i] == null) continue;
            if (!currentPrefabs[i].TryGetComponent(out Prefab_Controller controller)) continue;
            if (controller.prefabTag.prefabType == Prefab_Type.character) continue;
            if (controller.prefabTag.prefabID != objectID) continue;
            if (!ignoreMax)
            {
                if (controller.currentAmount >= _tilemapController.controller.prefabsData.Get_Item(objectID).maxAmount) continue;
            }
            return controller;
        }

        return null;
    }

    public int Prefab_Type_Amount(Prefab_Type type)
    {
        int count = 0;

        if (currentPrefabs.Count <= 0) return count;

        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (currentPrefabs[i] == null) continue;
            if (!currentPrefabs[i].TryGetComponent(out Prefab_Tag tag)) continue;
            if (type != tag.prefabType) continue;
            count ++;
        }
        return count;
    }

    // Tile Control
    public Sprite Random_Sprite()
    {
        if (_sprites.Count <= 0)
        {
            Debug.Log("No sprites in _sprites list!");
            return null;
        }

        if (_sprites.Count <= 1) return _sprites[0];

        int randNum = Random.Range(0, _sprites.Count);
        return _sprites[randNum];
    }

    public void Update_Position(int row, int column)
    {
        rowNum = row;
        columnNum = column;
    }
    public void Set_Data(TileMap_Controller mapController)
    {
        this.tilemapController = mapController;
        _sr.sprite = Random_Sprite();
    }

    public void Change_Type(int prefabID)
    {
        Prefabs_Data data = tilemapController.controller.prefabsData;

        GameObject tile = data.Get_Tile(prefabID);
        Prefab_Tag prefabTag;
        Tile_Controller tileController;

        // get prefab tag component
        if (!tile.TryGetComponent(out Prefab_Tag x)) return;
        prefabTag = x;

        // get tile controller component
        if (!tile.TryGetComponent(out Tile_Controller y)) return;
        tileController = y;

        // prefab tag type
        this.prefabTag.prefabType = prefabTag.prefabType;
        // prefab tag id
        this.prefabTag.prefabID = prefabTag.prefabID;
        // sprite
        _sr.sprite = tileController.Random_Sprite();
    }
    public void Change_Random()
    {
        Prefabs_Data data = tilemapController.controller.prefabsData;

        GameObject tile = data.Get_Random_Tile();
        Prefab_Tag prefabTag;
        Tile_Controller tileController;

        // get prefab tag component
        if (!tile.TryGetComponent(out Prefab_Tag x)) return;
        prefabTag = x;

        // get tile controller component
        if (!tile.TryGetComponent(out Tile_Controller y)) return;
        tileController = y;

        // prefab tag type
        this.prefabTag.prefabType = prefabTag.prefabType;
        // prefab tag id
        this.prefabTag.prefabID = prefabTag.prefabID;
        // sprite
        _sr.sprite = tileController.Random_Sprite();
    }
    public void Change_Adapt(bool overlapOnly)
    {
        List<Tile_Controller> surroundingTiles = tilemapController.combinationSystem.Surrounding_Tiles(rowNum, columnNum);

        if (overlapOnly)
        {
            // check if there is at least 1 overlap placeable tile
            bool overlapTileDetected = false;

            for (int i = 0; i < surroundingTiles.Count; i++)
            {
                if (!surroundingTiles[i].Is_Prefab_Type(Prefab_Type.overlapPlaceable)) continue;
                overlapTileDetected = true;
                break;
            }

            if (overlapTileDetected)
            {
                // remove all placeables
                for (int i = surroundingTiles.Count - 1; i >= 0; i--)
                {
                    if (surroundingTiles[i].prefabTag.prefabType != Prefab_Type.placeable) continue;
                    surroundingTiles.RemoveAt(i);
                }
            }
        }

        Tile_Controller targetTile = surroundingTiles[Random.Range(0, surroundingTiles.Count)];
        Change_Type(targetTile.prefabTag.prefabID);
    }

    // Prefab Control
    public void Set_Prefab(Transform prefabTransform)
    {
        prefabTransform.parent = _prefabsParent;
    }
    public void Remove_Prefab(Prefab_Type type, int searchID)
    {
        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (!currentPrefabs[i].TryGetComponent(out Prefab_Tag prefabTag)) continue;
            if (prefabTag.prefabType != type) continue;
            if (prefabTag.prefabID != searchID) continue;

            Destroy(currentPrefabs[i]);
            currentPrefabs.Clear();
            break;
        }

        currentPrefabs.RemoveAll(item => item == null);
    }

    // Update
    public void Update_Data()
    {
        Update_Current_Prefabs();
    }
    private void Update_Current_Prefabs()
    {
        currentPrefabs.Clear();

        if (_prefabsParent.childCount <= 0) return;

        for (int i = 0; i < _prefabsParent.childCount; i++)
        {
            currentPrefabs.Add(_prefabsParent.GetChild(i).gameObject);
        }
    }

    // Function
    public void Click()
    {
        if (_moveReady)
        {
            _tilemapController.actionSystem.Reset_NewMap_Directions();
            _tilemapController.actionSystem.Move_Player(this);
        }
        else if (_objectReady)
        {
            _tilemapController.actionSystem.Interact_Object(this);
        }
        else if (_itemDropReady)
        {
            _tilemapController.actionSystem.Drop_Item(this);
            if (_tilemapController.controller.inventoryController.dragSlot.itemDragging) return;
        }

        _tilemapController.actionSystem.UnHighlight_All_tiles();
        _tilemapController.playerController.interactReady = false;
    }
    public void Pointer_Enter()
    {
        if (!_itemDropReady) return;
        _tilemapController.controller.inventoryController.dragSlot.tileDetected = true;
    }
    public void Pointer_Exit()
    {
        _tilemapController.controller.inventoryController.dragSlot.tileDetected = false;
    }

    public void Move_Highlight()
    {
        _moveReady = true;
        _sr.color = Color.green;
    }
    public void Object_Highlight()
    {
        _objectReady = true;
        objectInteractBox.SetActive(_objectReady);
    }
    public void ItemDrop_Highlight()
    {
        _itemDropReady = true;
        itemDropkBox.SetActive(_itemDropReady);
    }

    public void UnHighlight()
    {
        _moveReady = false;
        _objectReady = false;
        _itemDropReady = false;

        _sr.color = Color.white;
        objectInteractBox.SetActive(_objectReady);
        itemDropkBox.SetActive(_itemDropReady);
    }
}