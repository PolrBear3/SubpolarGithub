using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
}
public interface IInteractableUpdate
{
    void Interact_Update();
}
public interface IEquippable
{
    void Use();
}
public interface IDamageable
{
    void Damage(int damageAmount);
}

public class Prefabs_Data : MonoBehaviour
{
    [SerializeField] private GameObject mapController;

    [SerializeField] private GameObject customObject;

    [SerializeField] private Prefab_Tag[] characters;
    [SerializeField] private List<Prefab_Tag> objects = new List<Prefab_Tag>();
    [SerializeField] private Prefab_Tag[] tiles;

    [SerializeField] private Item_ScrObj[] items;

    public GameObject Get_MapController()
    {
        return mapController;
    }

    // Character
    public GameObject Get_Character(int id)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (id != characters[i].prefabID) continue;
            return characters[i].Prefab();
        }
        return null;
    }
    
    // Object and Item
    public void Add_CustomObject_toData(Prefab_Type type, int id)
    {
        if (!customObject.TryGetComponent(out Prefab_Tag tag)) return;

        customObject.name = Get_Item(id).itemName;
        Prefab_Tag addTag = tag;

        addTag.prefabType = type;
        addTag.prefabID = id;

        objects.Add(addTag);
    }

    public GameObject Get_Object(int id)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (id != objects[i].prefabID) continue;
            return objects[i].Prefab();
        }
        return null;
    }
    public Prefab_Tag Get_Object_PrefabTag(int id)
    {
        GameObject prefab = Get_Object(id);
        if (prefab == null) return null;
        if (!prefab.TryGetComponent(out Prefab_Tag prefabTag)) return null;
        return prefabTag;
    }
    public Prefab_Controller Get_Object_PrefabController(int id)
    {
        GameObject prefab = Get_Object(id);
        if (prefab == null) return null;
        if (!prefab.TryGetComponent(out Prefab_Controller prefabController)) return null;
        return prefabController;
    }

    public Item_ScrObj Get_Item(int id)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (id != items[i].id) continue;
            return items[i];
        }
        return null;
    }
    public Item_ScrObj Get_Item(List<Ingredient> ingredients)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (ingredients.Count != items[i].ingredients.Count) continue;

            int errorAmount = items[i].ingredients.Count;
            for (int j = 0; j < ingredients.Count; j++)
            {
                if (ingredients[j].ingredientItem != items[i].ingredients[j].ingredientItem) break;
                if (ingredients[j].amount < items[i].ingredients[j].amount) break;
                errorAmount--;
            }

            if (errorAmount <= 0) return items[i];
        }
        return null;
    }
    
    // Tile
    public GameObject Get_Tile(int id)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (id != tiles[i].prefabID) continue;
            return tiles[i].Prefab();
        }
        return null;
    }
    public GameObject Get_Random_Tile()
    {
        int RandomNum = Random.Range(0, tiles.Length);

        return tiles[RandomNum].Prefab();
    }
    public GameObject Get_Random_Overlap_Tile()
    {
        List<Prefab_Tag> overlapTiles = new List<Prefab_Tag>();

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].prefabType != Prefab_Type.overlapPlaceable) continue;
            overlapTiles.Add(tiles[i]);
        }

        int RandomNum = Random.Range(0, overlapTiles.Count);

        return overlapTiles[RandomNum].gameObject;
    }
}