using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction_Controller : MonoBehaviour
{
    [SerializeField] private Game_Controller _controller;
    public Game_Controller controller { get => _controller; set => _controller = value; }

    [SerializeField] private Image _interactIcon;

    //
    public void OnShortcutKey1()
    {
        Pickup_Object();
    }
    public void OnShortcutKey2()
    {
        Interact_Object();
    }
    public void OnShortcutKey3()
    {
        Use_Equipment();
    }

    // Button Functions
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
    public void Use_Equipment()
    {

    }

    // Button Icon Updates
    public void Update_Interact_Icon()
    {
        // get player current tile
        Tile_Controller playerTile = _controller.tilemapController.Get_Tile(Prefab_Type.character, 0);

        // check for overlap placeable object
        Prefab_Controller overlapObject = playerTile.Get_Current_Prefab(Prefab_Type.overlapPlaceable);

        // check for Iinteractable
        if (overlapObject == null || !playerTile.Has_Interactable())
        {
            // empty interaction icon
            _interactIcon.sprite = null;
            _interactIcon.color = Color.clear;
        }
        else
        {
            // update interaction icon
            _interactIcon.sprite = overlapObject.sr.sprite;
            _interactIcon.color = Color.white;
        }
    }
    public void Update_Equipment_Icon()
    {

    }
}