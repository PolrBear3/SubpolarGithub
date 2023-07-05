using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_CraftTable : MonoBehaviour, IInteractable, IInteractableUpdate, IDamageable
{
    private Prefab_Controller _controller;

    [SerializeField] private Mini_ItemIcon ingredient1;
    [SerializeField] private Mini_ItemIcon ingredient2;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { _controller = controller; }
    }

    public void Interact_Update()
    {
        Insert_Ingredient();
    }
    public void Interact()
    {
        Craft();
    }
    public void Damage(int damageAmount)
    {
        Get_Damaged(damageAmount);
        Health_Check();
    }

    // Get
    private Item_ScrObj Get_Craft_Item()
    {
        Prefabs_Data data = _controller.tilemapController.controller.prefabsData;

        List<Ingredient> targetIngredients = new List<Ingredient>();
        Ingredient checkIngredient1;
        Ingredient checkIngredient2;

        checkIngredient1.ingredientItem = ingredient1.currentItem;
        checkIngredient1.amount = ingredient1.currentAmount;
        if (checkIngredient1.ingredientItem != null) targetIngredients.Add(checkIngredient1);

        checkIngredient2.ingredientItem = ingredient2.currentItem;
        checkIngredient2.amount = ingredient2.currentAmount;
        if (checkIngredient2.ingredientItem != null) targetIngredients.Add(checkIngredient2);

        return data.Get_Item(targetIngredients);
    }

    // Functions
    private void Get_Damaged(int damageAmount)
    {
        _controller.healthController.Subtract_Current_LifeCount(damageAmount);
    }
    private void Health_Check()
    {
        if (_controller.healthController.currentLifeCount > 0) return;
        // drop ingredients 1 and 2
        // drop Basic CraftTable ingredients
        Destroy(gameObject);
    }

    private void Insert_Ingredient()
    {
        Tile_Controller currentTile = _controller.tilemapController.Get_Tile(_controller.currentRowNum, _controller.currentColumnNum);

        // save object
        Prefab_Controller ingredient;

        if (currentTile.Has_Prefab_Type(Prefab_Type.overlapPlaceable))
        {
            // save overlap
            ingredient = currentTile.Get_Current_Prefab(Prefab_Type.overlapPlaceable);
        }
        else
        {
            // save unplaceable
            ingredient = currentTile.Get_Current_Prefab(Prefab_Type.unplaceable);
        }

        // no ingredients inserted
        if (ingredient == null) return;
        
        // save item
        Item_ScrObj objectItem = _controller.tilemapController.controller.prefabsData.Get_Item(ingredient.prefabTag.prefabID);

        // insert item
        if (!ingredient1.hasItem || (ingredient1.currentItem == objectItem && !ingredient1.Is_Max_Amount()))
        {
            ingredient1.Assign_Item(objectItem, ingredient.currentAmount);
        }
        else if (!ingredient2.hasItem || (ingredient2.currentItem == objectItem && !ingredient2.Is_Max_Amount()))
        {
            ingredient2.Assign_Item(objectItem, ingredient.currentAmount);
        }
        else
        {
            Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;
            inventory.Add_Item(objectItem, ingredient.currentAmount);
        }

        // calculate left over
        if (ingredient1.LeftOver() > 0)
        {
            ingredient2.Assign_Item(ingredient1.currentItem, ingredient1.LeftOver());
            ingredient1.currentAmount -= ingredient1.LeftOver();
        }

        // remove overlap object
        currentTile.Remove_Prefab(Prefab_Type.overlapPlaceable);

        // remove unplacealbe object
        currentTile.Remove_Prefab(Prefab_Type.unplaceable);
    }
    private void Craft()
    {
        if (!ingredient1.hasItem && !ingredient2.hasItem) return;
        
        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;

        // craft successful
        if (Get_Craft_Item() != null)
        {
            inventory.Add_Item(Get_Craft_Item(), 1);
        }
        // craft fail
        else
        {
            if (ingredient1.hasItem) inventory.Add_Item(ingredient1.currentItem, ingredient1.currentAmount);
            if (ingredient2.hasItem) inventory.Add_Item(ingredient2.currentItem, ingredient2.currentAmount);
        }
        
        ingredient1.Clear_Item();
        ingredient2.Clear_Item();
    }
}