using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    [SerializeField] private List<GameObject> _currentPrefabs = new List<GameObject>();
    public List<GameObject> currentPrefabs { get => _currentPrefabs; set => _currentPrefabs = value; }

    [SerializeField] private GameObject objectBlinkBox;

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
        if (moveReady)
        {
            tilemapController.actionSystem.Reset_NewMap_Directions();
            tilemapController.actionSystem.Move_Player(this);
        }
        else if (objectReady)
        {
            tilemapController.actionSystem.Interact_Object(this);
        }

        tilemapController.actionSystem.UnHighlight_All_tiles();
    }

    public void Move_Highlight()
    {
        moveReady = true;
        _sr.color = Color.green;
    }
    public void Object_Highlight()
    {
        objectReady = true;
        objectBlinkBox.SetActive(objectReady);
    }
    public void UnHighlight()
    {
        moveReady = false;
        objectReady = false;

        _sr.color = Color.white;
        objectBlinkBox.SetActive(objectReady);
    }
}