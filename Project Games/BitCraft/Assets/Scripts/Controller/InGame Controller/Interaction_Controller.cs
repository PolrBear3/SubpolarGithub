using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction_Controller : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Game_Controller _controller;
    public Game_Controller controller { get => _controller; set => _controller = value; }

    [Header("Main")]
    [SerializeField] private Image _pickupIcon;
    [SerializeField] private Image _interactIcon;
    [SerializeField] private Image _equipmentIcon;

    //
    public void OnInteractionKey1()
    {
        Pickup_Object();
    }
    public void OnInteractionKey2()
    {
        Interact_Object();
    }
    public void OnInteractionKey3()
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

        Update_Pickup_Icon(true);
        Update_Equipment_Icon();
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
        if (_controller.inventoryController.dragSlot.itemDragging) return;

        _controller.tilemapController.playerController.prefabController.equipmentController.Update_Current_Equipments();

        Item_ScrObj equippedItem = _controller.inventoryController.equippedSlot.currentItem;
        if (equippedItem == null) return;

        Prefab_Controller itemObject = _controller.prefabsData.Get_Object_PrefabController(equippedItem.id);
        if (itemObject == null || !itemObject.TryGetComponent(out IEquippable checkEquipment)) return;

        Prefab_Controller player = _controller.tilemapController.playerController.prefabController;
        Prefab_Controller playerEquipment = player.equipmentController.currentEquipment;

        if (!playerEquipment.TryGetComponent(out IEquippable currentEquipment)) return;
        currentEquipment.Use();
    }

    // Icon Updates
    public void Update_Pickup_Icon(bool pickupSuccessful)
    {
        if (pickupSuccessful)
        {
            // deactivate pickup icon
            _pickupIcon.color = Color.clear;

            return;
        }

        // get player current tile
        Tile_Controller playerTile = _controller.tilemapController.Get_Tile(Prefab_Type.character, 0);

        // check for overlap placeable object
        Prefab_Controller overlapObject = playerTile.Get_Current_Prefab(Prefab_Type.overlapPlaceable);
        // check for pickup objects
        if (overlapObject == null || playerTile.Has_Interactable())
        {
            // deactivate pickup icon
            _pickupIcon.color = Color.clear;
        }
        else
        {
            // activate pickup icon
            _pickupIcon.color = Color.white;
        }
    }
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
        if (_controller.inventoryController.dragSlot.itemDragging || !_controller.inventoryController.equippedSlot.hasItem)
        {
            _equipmentIcon.sprite = null;
            _equipmentIcon.color = Color.clear;
            return;
        }

        Item_ScrObj equippedItem = _controller.inventoryController.equippedSlot.currentItem;
        Prefab_Controller itemObject = _controller.prefabsData.Get_Object_PrefabController(equippedItem.id);

        if (itemObject == null || !itemObject.TryGetComponent(out IEquippable equippable))
        {
            _equipmentIcon.sprite = null;
            _equipmentIcon.color = Color.clear;
        }
        else
        {
            _equipmentIcon.sprite = equippedItem.sprite;
            _equipmentIcon.color = Color.white;
        }
    }
}