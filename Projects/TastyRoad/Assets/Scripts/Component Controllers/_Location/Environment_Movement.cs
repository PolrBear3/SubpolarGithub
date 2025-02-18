using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment_Movement : MonoBehaviour
{
    private Transform _playerPosition;
    [SerializeField] private float farDistance;



    // UnityEngine
    private void Start()
    {
        Get_PlayerPosition();
    }

    private void Update()
    {
        Parallax_Movement();
    }



    // Settings
    private void Get_PlayerPosition()
    {
        List<GameObject> currentCharacters = Main_Controller.instance.currentCharacters;

        for (int i = 0; i < currentCharacters.Count; i++)
        {
            if (!currentCharacters[i].TryGetComponent(out Player_Controller player)) continue;

            _playerPosition = player.transform;
            break;
        }
    }



    // Control
    private void Parallax_Movement()
    {
        transform.position = new Vector2(_playerPosition.position.x * (farDistance * 0.1f), transform.position.y);
    }
}
