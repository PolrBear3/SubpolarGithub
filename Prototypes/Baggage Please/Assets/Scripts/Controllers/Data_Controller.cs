using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_Controller : MonoBehaviour
{
    [SerializeField] private GameObject _npcPrefab;
    public GameObject npcPrefab => _npcPrefab;

    [SerializeField] private GameObject _baggagePrefab;
    public GameObject baggagePrefab => _baggagePrefab;
}
