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
        List<Tile_Controller> allTiles = tilemapController.currentMap.tiles;

        for (int i = 0; i < allTiles.Count; i++)
        {
            allTiles[i].UnHighlight();
        }
    }
    public void UnHighlight_All_EquipmentUseTiles()
    {
        Map_Controller currentMap = _tilemapController.currentMap;
        if (currentMap == null) return;

        _tilemapController.playerController.interactReady = false;

        List<Tile_Controller> allTiles = currentMap.tiles;

        for (int i = 0; i < allTiles.Count; i++)
        {
            allTiles[i].Equipment_Highlight_Activation(false);
        }
    }

    // Tile Highlight
    public void Highlight_Player_Interactable_Tiles()
    {
        UnHighlight_All_tiles();

        List<Tile_Controller> crossTiles = _tilemapController.combinationSystem.Cross_Tiles(Prefab_Type.character, 0);

        for (int i = 0; i < crossTiles.Count; i++)
        {
            // highlight moveable tiles
            if (!crossTiles[i].Is_Prefab_Type(Prefab_Type.placeable) && !crossTiles[i].Has_Prefab_Type(Prefab_Type.placeable))
            {
                if (crossTiles[i].Has_Prefab_Type(Prefab_Type.character)) continue;

                crossTiles[i].Move_Highlight_Activation(true);
            }

            // highlight interactable object tiles
            if (crossTiles[i].Has_Prefab_Type(Prefab_Type.placeable))
            {
                crossTiles[i].Object_Highlight_Activation(true);
            }
        }
    }
    public void Highlight_ItemDrop_Tiles()
    {
        UnHighlight_All_tiles();

        List<Tile_Controller> crossTiles = tilemapController.combinationSystem.Cross_Tiles(Prefab_Type.character, 0);

        int dragItemID = _tilemapController.controller.inventoryController.dragSlot.currentItem.id;

        Prefab_Controller dragObjectPrefab = _tilemapController.controller.prefabsData.Get_Object_PrefabController(dragItemID);

        // if dragging item is unplaceable
        if (dragObjectPrefab == null || dragObjectPrefab.prefabTag.prefabType == Prefab_Type.unplaceable)
        {
            for (int i = 0; i < crossTiles.Count; i++)
            {
                crossTiles[i].ItemDrop_Highlight_Activation(true);
            }
            return;
        }

        Prefab_Tag dragObjectTag = _tilemapController.controller.prefabsData.Get_Object_PrefabTag(dragItemID);

        for (int i = 0; i < crossTiles.Count; i++)
        {
            // object not drop available for this tile
            if (!dragObjectPrefab.Drop_Available(crossTiles[i].prefabTag.prefabID)) continue;

            // dragging item is placeable & tile has placeable object
            if (dragObjectTag.prefabType == Prefab_Type.placeable && crossTiles[i].Has_Prefab_Type(Prefab_Type.placeable)) continue;

            // if tile has overlapplaceable object and max amount
            Prefab_Controller tileCurrentPrefab = crossTiles[i].Get_Current_Prefab(dragItemID, false);
            if (crossTiles[i].Has_Prefab_Type(Prefab_Type.overlapPlaceable) && tileCurrentPrefab == null) continue;

            crossTiles[i].ItemDrop_Highlight_Activation(true);
        }
    }
    public void Highlight_EquipmentUse_Tiles(List<Tile_Controller> targetTiles)
    {
        UnHighlight_All_tiles();

        for (int i = 0; i < targetTiles.Count; i++)
        {
            targetTiles[i].Equipment_Highlight_Activation(true);
        }
    }

    // Map Direction
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

    // Tile Click Action
    public void Move_Player(Tile_Controller moveTile)
    {
        Player_Controller playerController = tilemapController.playerController;
        Prefab_Controller playerPrefabController = tilemapController.playerPrefabController;

        _tilemapController.controller.timeController.Update_Time(1);

        moveTile.Set_Prefab(playerController.transform);

        playerPrefabController.Update_Tile_Position(moveTile.rowNum, moveTile.columnNum);
        playerController.Move();
        playerPrefabController.Sprite_LayerOrder_Update(0);
        playerPrefabController.Sprite_Flip_Update();

        tilemapController.AllTiles_Update_Data();
        playerController.Click();

        _tilemapController.controller.interactionController.Update_Pickup_Icon(false);
        _tilemapController.controller.interactionController.Update_Interact_Icon();
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

        // player flip
        playerController.prefabController.Sprite_Flip_Update(tileWithObject.rowNum);
    }
    public void Drop_Item(Tile_Controller targetTile, int amount)
    {
        Drag_Slot dragSlot = _tilemapController.controller.inventoryController.dragSlot;

        int finalAmount = amount;

        // if dragSlot amount is less than bundle drop amount, change drop amount to dragslot current amount
        if (dragSlot.currentAmount < amount) finalAmount = dragSlot.currentAmount;

        _tilemapController.Set_Object(dragSlot.currentItem.id, finalAmount, targetTile.rowNum, targetTile.columnNum); 
        dragSlot.Decrease_Amount(finalAmount);

        // player flip
        Prefab_Controller player = _tilemapController.playerPrefabController;
        player.Sprite_Flip_Update(targetTile.rowNum);

        _tilemapController.controller.interactionController.Update_Equipment_Icon();
    }
    public void Use_Equipment(Tile_Controller targetTile)
    {
        Equipment_Controller playerEquipment = _tilemapController.playerController.prefabController.equipmentController;

        playerEquipment.Update_EquipmentUse_Tile(targetTile);

        if (!playerEquipment.currentEquipment.TryGetComponent(out IEquippable equippable)) return;
        equippable.Use();

        tilemapController.AllTiles_Update_Data();

        // player flip
        Prefab_Controller player = _tilemapController.playerPrefabController;
        player.Sprite_Flip_Update(targetTile.rowNum);
    }
}