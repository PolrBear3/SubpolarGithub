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
    private List<Map_Controller> _allMaps = new List<Map_Controller>();
    public List<Map_Controller> allMaps { get => _allMaps; set => _allMaps = value; }

    private Map_Controller _currentMap;
    public Map_Controller currentMap { get => _currentMap; set => _currentMap = value; }

    private Player_Controller _playerController;
    public Player_Controller playerController { get => _playerController; set => _playerController = value; }

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Action_System actionSystem)) { this.actionSystem = actionSystem; }
        if (gameObject.TryGetComponent(out TileMap_Render_System renderSystem)) { this.renderSystem = renderSystem; }
        if (gameObject.TryGetComponent(out TileMap_Combination_System combinationSystem)) { this.combinationSystem = combinationSystem; }
    }
    private void Start()
    {
        renderSystem.Set_New_Map(5);
        Set_Player(2, 2, true);
    }

    // map check
    public bool Has_Map(int positionX, int positionY)
    {
        for (int i = 0; i < allMaps.Count; i++)
        {
            if (allMaps[i].positionX != positionX) continue;
            if (allMaps[i].positionY != positionY) continue;
            return true;
        }
        return false;
    }
    public bool Has_Map(Map_Controller mapController)
    {
        for (int i = 0; i < allMaps.Count; i++)
        {
            if (allMaps[i] != mapController) continue;
            return true;
        }
        return false;
    }
    // map data
    public Map_Controller Get_Map(int positionX, int positionY)
    {
        for (int i = 0; i < allMaps.Count; i++)
        {
            if (allMaps[i].positionX == positionX && allMaps[i].positionY == positionY) return allMaps[i];
        }
        return null;
    }

    // tile data
    public Tile_Controller Get_Tile(int rowNum, int columnNum)
    {
        for (int i = 0; i < currentMap.tiles.Count; i++)
        {
            if (!currentMap.tiles[i].Found(rowNum, columnNum)) continue;
            return currentMap.tiles[i];
        }
        return null;
    }
    public Tile_Controller Get_Tile(Prefab_Type type, int prefabID)
    {
        for (int i = 0; i < currentMap.tiles.Count; i++)
        {
            if (!currentMap.tiles[i].Has_Prefab_ID(type, prefabID)) continue;
            return currentMap.tiles[i];
        }
        return null;
    }

    public List<Tile_Controller> Get_Tiles(Prefab_Type type, int prefabID)
    {
        List<Tile_Controller> returnList = new List<Tile_Controller>();

        for (int i = 0; i < currentMap.tiles.Count; i++)
        {
            if (!currentMap.tiles[i].Has_Prefab_ID(type, prefabID)) continue;
            returnList.Add(currentMap.tiles[i]);
        }

        return returnList;
    }

    // public functions
    public void AllTiles_Update_Data()
    {
        for (int i = 0; i < currentMap.tiles.Count; i++)
        {
            currentMap.tiles[i].Update_Data();
        }
    }

    public void Set_Player_Tile(bool newPlayer)
    {
        Tile_Controller playerTile = Get_Tile(playerController.currentRowNum, playerController.currentColumnNum);

        // only the player tile
        playerTile.Change_Adapt(true);

        if (!newPlayer) return;

        // player surrounding tiles
        List<Tile_Controller> surroundingTiles = combinationSystem.Surrounding_Tiles(playerTile);

        for (int i = 0; i < surroundingTiles.Count; i++)
        {
            surroundingTiles[i].Change_Adapt(true);
        }
    }
    public void Set_Player(int rowNum, int columnNum, bool newPlayer)
    {
        GameObject playerPrefab = controller.prefabsController.Get_Character(0);
        Tile_Controller playerTile = Get_Tile(rowNum, columnNum);

        // spawn
        GameObject player = Instantiate(playerPrefab, playerTile.transform.position, Quaternion.identity);

        // set player controller component
        if (player.TryGetComponent(out Player_Controller playerController)) { this.playerController = playerController; }

        // set player data
        playerController.Set_Data(this);

        // update player map position
        playerController.Update_Map_Position(currentMap.positionX, currentMap.positionY);

        // update player tiles position
        playerController.Update_Tile_Position(rowNum, columnNum);

        // place inside tile
        playerTile.Set_Prefab(player.transform);

        // update tiles
        AllTiles_Update_Data();
        
        // set player surrounding tiles to type overlap
        Set_Player_Tile(newPlayer);
    }
}
