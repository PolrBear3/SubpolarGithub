using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Prefab_Type
{
    none,
    all,
    character,
    placeable,
    overlapPlaceable,
    unplaceable
}

public class Prefab_Tag : MonoBehaviour
{
    [SerializeField] private Prefab_Type _prefabType;
    public Prefab_Type prefabType { get => _prefabType; set => _prefabType = value; }

    [SerializeField] private int _prefabID;
    public int prefabID { get => _prefabID; set => _prefabID = value; }

    [SerializeField] private int _layerOrderNum;
    public int layerOrderNum { get => _layerOrderNum; set => _layerOrderNum = value; }

    private GameObject _thisPrefab;
    public GameObject thisPrefab { get => _thisPrefab; set => _thisPrefab = value; }

    public GameObject Prefab()
    {
        thisPrefab = gameObject;
        return thisPrefab;
    }
}
