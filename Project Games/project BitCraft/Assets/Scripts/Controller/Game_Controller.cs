using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    private Prefabs_Controller _prefabsController;
    public Prefabs_Controller prefabsController { get => _prefabsController; set => _prefabsController = value; }

    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefabs_Controller prefabsController))
        {
            _prefabsController = prefabsController;
        }
    }
}