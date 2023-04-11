using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefab_Tag : MonoBehaviour
{
    [SerializeField] private int _prefabID;
    public int prefabID { get => _prefabID; set => _prefabID = value; }

    private GameObject _thisPrefab;
    public GameObject thisPrefab { get => _thisPrefab; set => _thisPrefab = value; }

    public GameObject Prefab()
    {
        thisPrefab = gameObject;
        return thisPrefab;
    }
}
