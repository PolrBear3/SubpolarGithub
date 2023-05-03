using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile_Controller : MonoBehaviour
{
    private SpriteRenderer _sr;

    [SerializeField] private List<Sprite> _sprites = new List<Sprite>(); 

    private TileMap_Controller _mapController;
    public TileMap_Controller mapController { get => _mapController; set => _mapController = value; }

    private Tile_Direction_System _directionSystem;
    public Tile_Direction_System directionSystem { get => _directionSystem; set => _directionSystem = value; }

    private Prefab_Tag _prefabTag;
    public Prefab_Tag prefabTag { get => _prefabTag; set => _prefabTag = value; }

    private int _rowNum;
    public int rowNum { get => _rowNum; set => _rowNum = value; }

    private int _columnNum;
    public int columnNum { get => _columnNum; set => _columnNum = value; }

    private bool _selectReady = false;
    public bool selectReady { get => _selectReady; set => _selectReady = value; }

    [SerializeField] private List<GameObject> _currentPrefabs = new List<GameObject>();
    public List<GameObject> currentPrefabs { get => _currentPrefabs; set => _currentPrefabs = value; }

    [SerializeField] private Transform _prefabsParent;

    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
        if (gameObject.TryGetComponent(out Prefab_Tag prefabTag)) { this.prefabTag = prefabTag; }
    }

    // checks
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

    // tile control
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
        this.mapController = mapController;
        _sr.sprite = Random_Sprite();
    }
    public void Change_Type_ID(int prefabID)
    {
        Prefabs_Controller controller = mapController.controller.prefabsController;

        GameObject tile = controller.Get_Tile(prefabID);
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
        Prefabs_Controller controller = mapController.controller.prefabsController;

        GameObject tile = controller.Get_Random_Tile();
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
    public void Change_Random_Overlap()
    {
        Prefabs_Controller controller = mapController.controller.prefabsController;

        GameObject tile = controller.Get_Random_Overlap_Tile();
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

    // prefab control
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

    // updates
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

    // functions
    public void Click()
    {
        if (!selectReady) return;

        mapController.actionSystem.Reset_NewMap_Directions();

        // tile click interactions
        mapController.actionSystem.Move_Player(this);
        //

        mapController.actionSystem.UnHighlight_All_tiles();
    }

    public void Highlight()
    {
        selectReady = true;
        _sr.color = Color.green;
    }
    public void UnHighlight()
    {
        selectReady = false;
        _sr.color = Color.white;
    }
}