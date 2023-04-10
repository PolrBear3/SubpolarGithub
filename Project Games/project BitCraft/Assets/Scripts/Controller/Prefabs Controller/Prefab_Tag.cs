using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefab_Tag : MonoBehaviour
{
    [SerializeField] private int prefabID;
    private GameObject thisPrefab;

    public int Prefab_ID() 
    {
        return prefabID;
    }

    public GameObject Prefab()
    {
        thisPrefab = gameObject;
        return thisPrefab;
    }
}
