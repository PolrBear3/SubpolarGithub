using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Interaction : MonoBehaviour
{
    [HideInInspector] public Player_Controller _playerController;

    //
    private Food _currentFood;
    [SerializeField] private SpriteRenderer _foodSR;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Player_Controller playerController)) _playerController = playerController;
    }

    //

}
