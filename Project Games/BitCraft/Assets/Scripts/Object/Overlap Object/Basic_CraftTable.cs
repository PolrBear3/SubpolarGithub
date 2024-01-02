using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_CraftTable : MonoBehaviour, IInteractable, IInteractableUpdate, IDamageable
{
    private LeanTween_AnimationController _animController;
    private Prefab_Controller _controller;

    [SerializeField] private GameObject _ingredientBox1;
    [SerializeField] private GameObject _ingredientBox2;

    [SerializeField] private Mini_ItemIcon _ingredient1;
    [SerializeField] private Mini_ItemIcon _ingredient2;

    [SerializeField] private SpriteRenderer _ingredientBox1Sprite;
    [SerializeField] private SpriteRenderer _ingredientBox2Sprite;

    [SerializeField] private Sprite _whiteBox;
    [SerializeField] private Sprite _greenBox;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out LeanTween_AnimationController animController)) _animController = animController;
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

        _animController.Interact_Animation();
    }

    // Get
    private Item_ScrObj Get_Craft_Item()
    {
        Prefabs_Data data = _controller.tilemapController.controller.prefabsData;

        List<Ingredient> targetIngredients = new List<Ingredient>();
        Ingredient checkIngredient1;
        Ingredient checkIngredient2;

        checkIngredient1.ingredientItem = _ingredient1.currentItem;
        checkIngredient1.amount = _ingredient1.currentAmount;
        if (checkIngredient1.ingredientItem != null) targetIngredients.Add(checkIngredient1);

        checkIngredient2.ingredientItem = _ingredient2.currentItem;
        checkIngredient2.amount = _ingredient2.currentAmount;
        if (checkIngredient2.ingredientItem != null) targetIngredients.Add(checkIngredient2);

        return data.Get_Item(targetIngredients);
    }

    // Basic Functions
    private void Get_Damaged(int damageAmount)
    {
        _controller.statController.Update_Current_Life(-damageAmount);
    }
    private void Health_Check()
    {
        if (_controller.statController.currentLifeCount > 0) return;

        Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;

        // return ingredients 1 and 2
        inventory.Add_Item(_ingredient1.currentItem, _ingredient1.currentAmount);
        inventory.Add_Item(_ingredient2.currentItem, _ingredient2.currentAmount);

        // return Basic CraftTable ingredients

        _controller.Destroy_Prefab();
    }

    // Crafting
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
        if (ingredient == null)
        {
            IngredientBox_CraftItemFound_Update();
            return;
        }
        
        // save item
        Item_ScrObj objectItem = _controller.tilemapController.controller.prefabsData.Get_Item(ingredient.prefabTag.prefabID);

        // insert item
        if (!_ingredient1.hasItem || (_ingredient1.currentItem == objectItem && !_ingredient1.Is_Max_Amount()))
        {
            _ingredient1.Assign_Item(objectItem, ingredient.currentAmount);
            IngredientBox_Activation(1, true);
        }
        else if (!_ingredient2.hasItem || (_ingredient2.currentItem == objectItem && !_ingredient2.Is_Max_Amount()))
        {
            _ingredient2.Assign_Item(objectItem, ingredient.currentAmount);
            IngredientBox_Activation(2, true);
        }
        else
        {
            Inventory_Controller inventory = _controller.tilemapController.controller.inventoryController;
            inventory.Add_Item(objectItem, ingredient.currentAmount);
        }

        // calculate left over
        if (_ingredient1.LeftOver() > 0)
        {
            _ingredient2.Assign_Item(_ingredient1.currentItem, _ingredient1.LeftOver());
            IngredientBox_Activation(2, true);

            _ingredient1.currentAmount -= _ingredient1.LeftOver();
        }

        // remove overlap object
        currentTile.Remove_Prefab(Prefab_Type.overlapPlaceable);

        // remove unplacealbe object
        currentTile.Remove_Prefab(Prefab_Type.unplaceable);

        // craft item available check
        IngredientBox_CraftItemFound_Update();
    }
    private void Use_Ingredient()
    {
        List<Ingredient> itemIngredients = Get_Craft_Item().ingredients;

        for (int i = 0; i < itemIngredients.Count; i++)
        {
            if (_ingredient1.currentItem == itemIngredients[i].ingredientItem)
            {
                _ingredient1.currentAmount -= itemIngredients[i].amount;
                if (_ingredient1.currentAmount <= 0)
                {
                    _ingredient1.Clear_Item();
                    IngredientBox_Activation(1, false);
                }
                continue;
            }
            if (_ingredient2.currentItem == itemIngredients[i].ingredientItem)
            {
                _ingredient2.currentAmount -= itemIngredients[i].amount;
                if (_ingredient2.currentAmount <= 0)
                {
                    _ingredient2.Clear_Item();
                    IngredientBox_Activation(2, false);
                }
                continue;
            }
        }
    }
    private void Craft()
    {
        Craft_Bounce();

        if (!_ingredient1.hasItem && !_ingredient2.hasItem) return;
        
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
            if (_ingredient1.hasItem && !inventory.Is_Inventory_Full(_ingredient1.currentItem, _ingredient1.currentAmount))
            {
                inventory.Add_Item(_ingredient1.currentItem, _ingredient1.currentAmount);
                _ingredient1.Clear_Item();
                IngredientBox_Activation(1, false);
            }

            if (_ingredient2.hasItem && !inventory.Is_Inventory_Full(_ingredient2.currentItem, _ingredient2.currentAmount))
            {
                inventory.Add_Item(_ingredient2.currentItem, _ingredient2.currentAmount);
                _ingredient2.Clear_Item();
                IngredientBox_Activation(2, false);
            }
        }
    }

    // Visual
    private void IngredientBox_Activation(int boxNum, bool activate)
    {
        if (boxNum == 1)
        {
            if (activate)
            {
                _ingredientBox1Sprite.color = Color.white;
                return;
            }

            _ingredientBox1Sprite.color = Color.clear;
        }
        else
        {
            if (activate)
            {
                _ingredientBox2Sprite.color = Color.white;
                return;
            }

            _ingredientBox2Sprite.color = Color.clear;
        }
    }
    private void IngredientBox_CraftItemFound_Update()
    {
        if (Get_Craft_Item() != null)
        {
            _ingredientBox1Sprite.sprite = _greenBox;
            _ingredientBox2Sprite.sprite = _greenBox;
        }
        else
        {
            _ingredientBox1Sprite.sprite = _whiteBox;
            _ingredientBox2Sprite.sprite = _whiteBox;
        }
    }

    // Animation
    private void Craft_Bounce()
    {
        // up
        LeanTween.moveLocal(_ingredientBox1, new Vector2(-0.22f, 0.55f), 0.2f).setEase(LeanTweenType.easeInOutQuint);
        LeanTween.moveLocal(_ingredientBox2, new Vector2(0.22f, 0.55f), 0.2f).setDelay(0.1f).setEase(LeanTweenType.easeInOutQuint);

        // down
        LeanTween.moveLocal(_ingredientBox1, new Vector2(-0.22f, 0.5f), 0.2f).setDelay(0.2f).setEase(LeanTweenType.easeInOutQuint).setEase(LeanTweenType.easeInOutQuint);
        LeanTween.moveLocal(_ingredientBox2, new Vector2(0.22f, 0.5f), 0.2f).setDelay(0.3f);
    }
}