using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs_Controller : MonoBehaviour
{
    [SerializeField] private Prefab_Tag[] characters;
    [SerializeField] private Prefab_Tag[] tiles;

    public GameObject Get_Character(int id)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (id != characters[i].prefabID) continue;
            return characters[i].Prefab();
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