using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap_Controller : MonoBehaviour
{
    private Main_Controller _mainController;

    [SerializeField] private GameObject _cursor;



    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
    }



    //
}
