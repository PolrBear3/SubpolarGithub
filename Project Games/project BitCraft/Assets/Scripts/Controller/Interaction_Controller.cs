using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction_Controller : MonoBehaviour
{
    [SerializeField] private Game_Controller _controller;
    public Game_Controller controller { get => _controller; set => _controller = value; }

    //
    public void OnShortcutKey1()
    {
        Pickup_Object();
    }
    public void OnShortcutKey2()
    {
        Interact_Object();
    }

    // Interaction Button Functions
    public void Pickup_Object()
    {
        // get player position tile
        Tile_Controller playerTile = _controller.tilemapController.Get_Tile(Prefab_Type.character, 0);

        // check for object
        Prefab_Controller objectPrefab = playerTile.Get_Current_Prefab(Prefab_Type.overlapPlaceable);
        if (objectPrefab == null) return;

        // check for item
        Item_ScrObj objectItem = _controller.prefabsData.Get_Item(objectPrefab.prefabTag.prefabID);
        if (objectItem == null) return;

        // add item to inventory
        _controller.inventoryController.Add_Item(objectItem, objectPrefab.currentAmount);

        // remove item object from tile
        playerTile.Remove_Prefab(Prefab_Type.overlapPlaceable);
    }
    public void Interact_Object()
    {
        // get player position tile
        Tile_Controller playerTile = _controller.tilemapController.Get_Tile(Prefab_Type.character, 0);

        // check for interactable object
        Prefab_Controller objectPrefab = playerTile.Get_Current_Prefab(Prefab_Type.overlapPlaceable);
        if (objectPrefab == null) return;
        if (!objectPrefab.TryGetComponent(out IInteractable interactable)) return;

        // activate interactable object
        interactable.Interact();
    }
}
