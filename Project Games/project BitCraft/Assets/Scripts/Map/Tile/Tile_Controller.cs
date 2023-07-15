using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile_Controller : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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

    private bool _equipmentUseReady = false;
    public bool equipmentUseReady { get => _equipmentUseReady; set => _equipmentUseReady = value; }

    [Header("Current Prefabs Data")]
    [SerializeField] private Transform _prefabsParent;

    [SerializeField] private List<Prefab_Controller> _currentPrefabs = new List<Prefab_Controller>();
    public List<Prefab_Controller> currentPrefabs { get => _currentPrefabs; set => _currentPrefabs = value; }

    [Header("Interaction Box")]
    [SerializeField] private GameObject _objectInteractBox;
    [SerializeField] private GameObject _itemDropkBox;
    [SerializeField] private GameObject _equipmentUseBox;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
        if (gameObject.TryGetComponent(out Prefab_Tag prefabTag)) { this.prefabTag = prefabTag; }
    }
    public void Update_Data()
    {
        Update_CurrentPrefabs();
        Activate_IInteractableUpdates();
        Remove_Prefab(Prefab_Type.unplaceable);
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
            if (searchID != currentPrefabs[i].prefabTag.prefabID) continue;
            return true;
        }
        return false;
    }

    public bool Has_Interactable()
    {
        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (!currentPrefabs[i].TryGetComponent(out IInteractable interactable)) continue;
            return true;
        }
        return false;
    }

    // Get
    public Prefab_Controller Get_Current_Prefab(Prefab_Type prefabType)
    {
        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (!Has_Prefab_Type(prefabType)) continue;
            if (prefabType != currentPrefabs[i].prefabTag.prefabType) continue;
            return currentPrefabs[i];
        }
        return null;
    }
    public Prefab_Controller Get_Current_Prefab(int id, bool ignoreMax)
    {
        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (id != currentPrefabs[i].prefabTag.prefabID) continue;
            if (!ignoreMax)
            {
                int maxAmount = _tilemapController.controller.prefabsData.Get_Item(id).maxAmount;
                if (currentPrefabs[i].currentAmount >= maxAmount) continue;
            }
            return currentPrefabs[i];
        }
        return null;
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

    public void Remove_Prefab(Prefab_Type type)
    {
        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (currentPrefabs[i].prefabTag.prefabType != type) continue;
            Destroy(currentPrefabs[i].gameObject);
            currentPrefabs.Clear();
            break;
        }

        Update_CurrentPrefabs();
    }
    public void Remove_Prefab(Prefab_Type type, int searchID)
    {
        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (currentPrefabs[i].prefabTag.prefabType != type) continue;
            if (currentPrefabs[i].prefabTag.prefabID != searchID) continue;
            Destroy(currentPrefabs[i].gameObject);
            currentPrefabs.Clear();
            break;
        }

        Update_CurrentPrefabs();
    }

    // Update
    private void Update_CurrentPrefabs()
    {
        currentPrefabs.RemoveAll(item => item == null);
        currentPrefabs.Clear();

        if (_prefabsParent.childCount <= 0) return;

        for (int i = 0; i < _prefabsParent.childCount; i++)
        {
            if (!_prefabsParent.GetChild(i).TryGetComponent(out Prefab_Controller controller)) continue;
            currentPrefabs.Add(controller);
        }
    }
    private void Activate_IInteractableUpdates()
    {
        for (int i = 0; i < currentPrefabs.Count; i++)
        {
            if (!currentPrefabs[i].TryGetComponent(out IInteractableUpdate interactableUpdate)) continue;
            interactableUpdate.Interact_Update();
        }
    }

    // Function
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_itemDropReady) return;
        _tilemapController.controller.inventoryController.dragSlot.tileDetected = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _tilemapController.controller.inventoryController.dragSlot.tileDetected = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Drag_Slot dragSlot = _tilemapController.controller.inventoryController.dragSlot;

        // left mouse click
        if (eventData.button == PointerEventData.InputButton.Left)
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
                _tilemapController.actionSystem.Drop_Item(this, dragSlot.bundleDropAmount);
                
                if (dragSlot.itemDragging)
                {
                    _tilemapController.actionSystem.Highlight_ItemDrop_Tiles();
                    return;
                }
            }
            else if (_equipmentUseReady)
            {
                Update_Data();
                _tilemapController.actionSystem.Use_Equipment(this);
            }
        }
        // right mouse click
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_itemDropReady)
            {
                _tilemapController.actionSystem.Drop_Item(this, 1);
                if (dragSlot.itemDragging) return;
            }
        }

        _tilemapController.actionSystem.UnHighlight_All_tiles();
        _tilemapController.playerController.interactReady = false;
    }

    public void Move_Highlight_Activation(bool activate)
    {
        _moveReady = activate;

        if (activate) _sr.color = Color.green;
        else _sr.color = Color.white;
    }
    public void Object_Highlight_Activation(bool activate)
    {
        _objectReady = activate;
        _objectInteractBox.SetActive(activate);
    }
    public void ItemDrop_Highlight_Activation(bool activate)
    {


        _itemDropReady = activate;
        _itemDropkBox.SetActive(activate);
    }
    public void Equipment_Highlight_Activation(bool activate)
    {
        _equipmentUseReady = activate;
        _equipmentUseBox.SetActive(activate);
    }

    public void UnHighlight()
    {
        Move_Highlight_Activation(false);
        Object_Highlight_Activation(false);
        ItemDrop_Highlight_Activation(false);
        Equipment_Highlight_Activation(false);

        _directionSystem.Reset_Directions();
    }
}