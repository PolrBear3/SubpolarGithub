using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_CraftTable : MonoBehaviour, IInteractable, IInteractableUpdate, IDamageable
{
    private Prefab_Controller _controller;

    [SerializeField] private Mini_ItemIcon ingredient1;
    [SerializeField] private Mini_ItemIcon ingredient2;

    [SerializeField] private SpriteRenderer ingredientBox1;
    [SerializeField] private SpriteRenderer ingredientBox2;

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

    private void IngredientBox_Activation(int boxNum, bool activate)
    {
        if (boxNum == 1)
        {
            if (activate)
            {
                ingredientBox1.color = Color.white;
                return;
            }
            ingredientBox1.color = Color.clear;
        }
        else
        {
            if (activate)
            {
                ingredientBox2.color = Color.white;
                return;
            }
            ingredientBox2.color = Color.clear;
        }
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
            IngredientBox_Activation(1, true);
        }
        else if (!ingredient2.hasItem || (ingredient2.currentItem == objectItem && !ingredient2.Is_Max_Amount()))
        {
            ingredient2.Assign_Item(objectItem, ingredient.currentAmount);
            IngredientBox_Activation(2, true);
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
            IngredientBox_Activation(2, true);
            ingredient1.currentAmount -= ingredient1.LeftOver();
        }

        // remove overlap object
        currentTile.Remove_Prefab(Prefab_Type.overlapPlaceable);

        // remove unplacealbe object
        currentTile.Remove_Prefab(Prefab_Type.unplaceable);
    }
    private void Use_Ingredient()
    {
        List<Ingredient> itemIngredients = Get_Craft_Item().ingredients;

        for (int i = 0; i < itemIngredients.Count; i++)
        {
            if (ingredient1.currentItem == itemIngredients[i].ingredientItem)
            {
                ingredient1.currentAmount -= itemIngredients[i].amount;
                if (ingredient1.currentAmount <= 0)
                {
                    ingredient1.Clear_Item();
                    IngredientBox_Activation(1, false);
                }
                continue;
            }
            if (ingredient2.currentItem == itemIngredients[i].ingredientItem)
            {
                ingredient2.currentAmount -= itemIngredients[i].amount;
                if (ingredient2.currentAmount <= 0)
                {
                    ingredient2.Clear_Item();
                    IngredientBox_Activation(2, false);
                }
                continue;
            }
        }
    }
    private void Craft()
    {
        if (!ingredient1.hasItem && !ingredient2.hasItem) return;
        
        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;
        Item_ScrObj craftItem = Get_Craft_Item();

        // craft successful
        if (craftItem != null && !inventory.Is_Inventory_Full(craftItem, 1))
        {
            inventory.Add_Item(craftItem, 1);
            Use_Ingredient();
        }
        // craft fail
        else
        {
            if (ingredient1.hasItem && !inventory.Is_Inventory_Full(ingredient1.currentItem, ingredient1.currentAmount))
            {
                inventory.Add_Item(ingredient1.currentItem, ingredient1.currentAmount);
                ingredient1.Clear_Item();
                IngredientBox_Activation(1, false);
            }

            if (ingredient2.hasItem && !inventory.Is_Inventory_Full(ingredient2.currentItem, ingredient2.currentAmount))
            {
                inventory.Add_Item(ingredient2.currentItem, ingredient2.currentAmount);
                ingredient2.Clear_Item();
                IngredientBox_Activation(2, false);
            }
        }
    }
}