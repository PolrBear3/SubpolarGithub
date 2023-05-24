using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
}

public class Prefabs_Data : MonoBehaviour
{
    [SerializeField] private GameObject mapController;

    [SerializeField] private Prefab_Tag[] characters;
    [SerializeField] private Prefab_Tag[] objects;
    [SerializeField] private Prefab_Tag[] tiles;

    [SerializeField] private Item_ScrObj[] items;

    public GameObject Get_MapController()
    {
        return mapController;
    }

    public GameObject Get_Character(int id)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (id != characters[i].prefabID) continue;
            return characters[i].Prefab();
        }
        return null;
    }
    public GameObject Get_Object(int id)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (id != objects[i].prefabID) continue;
            return objects[i].Prefab();
        }
        return null;
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