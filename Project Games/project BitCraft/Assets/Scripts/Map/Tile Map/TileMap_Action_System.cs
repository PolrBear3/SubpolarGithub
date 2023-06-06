using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileMap_Action_System : MonoBehaviour
{
    private TileMap_Controller _tilemapController;
    public TileMap_Controller tilemapController { get => _tilemapController; set => _tilemapController = value; }

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out TileMap_Controller mapController)) { this.tilemapController = mapController; }
    }

    // public action systems
    public void UnHighlight_All_tiles()
    {
        List<Tile_Controller> tiles = tilemapController.currentMap.tiles;

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].UnHighlight();
        }
    }

    // player action systems
    public void Highlight_Player_Interactable_Tiles()
    {
        List<Tile_Controller> crossTiles = tilemapController.combinationSystem.Cross_Tiles(Prefab_Type.character, 0);

        for (int i = 0; i < crossTiles.Count; i++)
        {
            // highlight moveable tiles
            if (!crossTiles[i].Is_Prefab_Type(Prefab_Type.placeable) && !crossTiles[i].Has_Prefab_Type(Prefab_Type.placeable))
            {
                crossTiles[i].Move_Highlight();
            }

            // highlight interactable object tiles
            if (crossTiles[i].Has_Prefab_Type(Prefab_Type.placeable))
            {
                crossTiles[i].Object_Highlight();
            }
        }
    }
    public void Highlight_ItemDrop_Tiles()
    {
        UnHighlight_All_tiles();
        
        List<Tile_Controller> crossTiles = tilemapController.combinationSystem.Cross_Tiles(Prefab_Type.character, 0);

        int dragItemID = _tilemapController.controller.inventoryController.dragSlot.currentItem.id;
        
        Prefab_Controller dragObjectController = _tilemapController.controller.prefabsData.Get_Object_PrefabController(dragItemID);
        Prefab_Tag dragObjectTag = _tilemapController.controller.prefabsData.Get_Object_PrefabTag(dragItemID);

        for (int i = 0; i < crossTiles.Count; i++)
        {
            // drop available tile
            if (!dragObjectController.Drop_Available(crossTiles[i].prefabTag.prefabID)) continue;

            // dragging item is placeable & tile have placeable object
            if (dragObjectTag.prefabType == Prefab_Type.placeable && crossTiles[i].Has_Prefab_Type(Prefab_Type.placeable)) continue;

            // test
            Prefab_Controller checkPrefab = crossTiles[i].Current_Prefab(dragItemID, false);
            if (crossTiles[i].Is_PrefabsAmount_Max() && checkPrefab == null) continue;

            crossTiles[i].ItemDrop_Highlight();
        }
    }

    public void Set_NewMap_Directions()
    {
        Prefab_Controller playerPrefabController = tilemapController.playerPrefabController;
        Tile_Controller playerTile = tilemapController.Get_Tile(playerPrefabController.currentRowNum, playerPrefabController.currentColumnNum);

        playerTile.directionSystem.Set_Directions();
    }
    public void Reset_NewMap_Directions()
    {
        Prefab_Controller playerPrefabController = tilemapController.playerPrefabController;
        Tile_Controller playerTile = tilemapController.Get_Tile(playerPrefabController.currentRowNum, playerPrefabController.currentColumnNum);

        playerTile.directionSystem.Reset_Directions();
    }

    public void Move_Player(Tile_Controller moveTile)
    {
        Player_Controller playerController = tilemapController.playerController;
        Prefab_Controller playerPrefabController = tilemapController.playerPrefabController;

        moveTile.Set_Prefab(playerController.transform);

        playerPrefabController.Update_Tile_Position(moveTile.rowNum, moveTile.columnNum);
        playerController.Move();
        playerPrefabController.Sprite_LayerOrder_Update(0);

        tilemapController.AllTiles_Update_Data();
        playerController.Click();
    }
    public void Interact_Object(Tile_Controller tileWithObject)
    {
        Player_Controller playerController = tilemapController.playerController;

        for (int i = 0; i < tileWithObject.currentPrefabs.Count; i++)
        {
            // ignore destroyed object
            if (tileWithObject.currentPrefabs[i] == null) continue;

            // find Iinteractable object
            if (!tileWithObject.currentPrefabs[i].TryGetComponent(out IInteractable interactable)) continue;

            // activate interable object
            interactable.Interact();
        }

        tilemapController.AllTiles_Update_Data();
        playerController.Click();
    }
    public void Drop_Item(Tile_Controller targetTile, int amount)
    {
        Drag_Slot dragSlot = _tilemapController.controller.inventoryController.dragSlot;

        _tilemapController.Set_Object(dragSlot.currentItem.id, amount, targetTile.rowNum, targetTile.columnNum); 
        dragSlot.Decrease_Amount(amount);
    }
}