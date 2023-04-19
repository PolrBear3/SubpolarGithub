using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap_Controller : MonoBehaviour
{
    // components
    [SerializeField] private Game_Controller _controller;
    public Game_Controller controller { get => _controller; set => _controller = value; }

    private TileMap_Action_System _actionSystem;
    public TileMap_Action_System actionSystem { get => _actionSystem; set => _actionSystem = value; }

    private TileMap_Render_System _renderSystem;
    public TileMap_Render_System renderSystem { get => _renderSystem; set => _renderSystem = value; }

    private TileMap_Combination_System _combinationSystem;
    public TileMap_Combination_System combinationSystem { get => _combinationSystem; set => _combinationSystem = value; }

    // ingame components
    private List<Tile_Controller> _tiles = new List<Tile_Controller>();
    public List<Tile_Controller> tiles { get => _tiles; set => _tiles = value; }

    private Player_Controller _playerController;
    public Player_Controller playerController { get => _playerController; set => _playerController = value; }

    // data
    private int _mapSize;
    public int mapSize { get => _mapSize; set => _mapSize = value; }

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Action_System actionSystem)) { this.actionSystem = actionSystem; }
        if (gameObject.TryGetComponent(out TileMap_Render_System renderSystem)) { this.renderSystem = renderSystem; }
        if (gameObject.TryGetComponent(out TileMap_Combination_System combinationSystem)) { this.combinationSystem = combinationSystem; }
    }
    private void Start()
    {
        renderSystem.Set_Tiles(5);
        Set_Player(2, 2);
    }

    public Tile_Controller Get_Tile(int rowNum, int columnNum)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (!tiles[i].Found(rowNum, columnNum)) continue;
            return tiles[i];
        }
        return null;
    }
    public Tile_Controller Get_Tile_With_PrefabID(Prefab_Type type, int prefabID)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (!tiles[i].Has_Prefab_ID(type, prefabID)) continue;
            return _tiles[i];
        }
        return null;
    }
    public List<Tile_Controller> Get_Tiles_With_PrefabID(Prefab_Type type, int prefabID)
    {
        List<Tile_Controller> returnList = new List<Tile_Controller>();

        for (int i = 0; i < tiles.Count; i++)
        {
            if (!tiles[i].Has_Prefab_ID(type, prefabID)) continue;
            returnList.Add(tiles[i]);
        }

        return returnList;
    }

    public void AllTiles_Update_Data()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].Update_Data();
        }
    }

    private void Set_Player(int rowNum, int columnNum)
    {
        GameObject playerPrefab = controller.prefabsController.Get_Character(0);
        Transform tilePosition = Get_Tile(rowNum, columnNum).transform;

        // spawn
        GameObject player = Instantiate(playerPrefab, tilePosition.position, Quaternion.identity);

        // set player controller component
        if (player.TryGetComponent(out Player_Controller playerController)) { this.playerController = playerController; }

        // set player data
        playerController.Set_Data(this);

        // update player position
        playerController.Update_Position(rowNum, columnNum);

        // place inside tile
        Get_Tile(rowNum, columnNum).Set_Prefab(player.transform);

        // update tiles
        AllTiles_Update_Data();

        // set player surrounding tiles to default tiles
        List<Tile_Controller> surroundingTiles = combinationSystem.Prefab_Surrounding(Prefab_Type.character, 0);

        for (int i = 0; i < surroundingTiles.Count; i++)
        {
            surroundingTiles[i].Change_Type(0);
        }
    }
}
