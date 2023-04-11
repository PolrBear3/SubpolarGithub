using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap_Controller : MonoBehaviour
{
    // components
    [SerializeField] private Game_Controller controller;

    private TileMap_Combination_System _combinationSystem;
    public TileMap_Combination_System combinationSystem { get => _combinationSystem; set => _combinationSystem = value; }

    private TileMap_Action_System _actionSystem;
    public TileMap_Action_System actionSystem { get => _actionSystem; set => _actionSystem = value; }

    // ingame components
    private List<Tile_Controller> _tiles = new List<Tile_Controller>();
    public List<Tile_Controller> tiles { get => _tiles; set => _tiles = value; }

    private Player_Controller _playerController;
    public Player_Controller playerController { get => _playerController; set => _playerController = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Combination_System combinationSystem)) { this.combinationSystem = combinationSystem; }
        if (gameObject.TryGetComponent(out TileMap_Action_System actionSystem)) { this.actionSystem = actionSystem; }
    }
    private void Start()
    {
        Set_Tiles(5);
        Set_Player(0, 0);
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

    private void Set_Tiles(int size)
    {
        int rowNum = 0;
        int columnNum = 0;

        for (int i = 0; i < size * size; i++)
        {
            // set position
            Vector2 position = new(rowNum - 2, -columnNum + 2);

            // instantiate
            GameObject tile = Instantiate(controller.prefabsController.Get_Tile(0), position, Quaternion.identity);

            // set inside tile map as child
            tile.transform.parent = transform;

            // get tile controller component
            if (tile.TryGetComponent(out Tile_Controller tileData))
            {
                // add to tiles list
                tiles.Add(tileData);

                // update tile data
                tileData.Set_Data(rowNum, columnNum);
            }

            // tile num update
            rowNum++;

            if (rowNum <= 4) continue;

            rowNum = 0;
            columnNum++;
        }
    }
    private void Set_Player(int rowNum, int columnNum)
    {
        GameObject playerPrefab = controller.prefabsController.Get_Character(0);
        Transform tilePosition = Get_Tile(rowNum, columnNum).transform;

        // spawn
        GameObject player = Instantiate(playerPrefab, tilePosition.position, Quaternion.identity);

        // set player controller component
        if (player.TryGetComponent(out Player_Controller playerController))
        {
            this.playerController = playerController;
        }

        // update player data
        playerController.Set_Data(rowNum, columnNum);

        // place inside tile
        Get_Tile(rowNum, columnNum).Track_Current_Prefabs(player.transform);
    }
}
