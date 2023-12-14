using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Controller : MonoBehaviour
{
    private Game_Controller _gameController;

    [Header("Data")]
    public float spawnIntervalTime;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Game_Controller gameController)) { _gameController = gameController; }
    }
    private void Start()
    {
        StartCoroutine(Spawn_Customer(5));
    }

    // Custom
    public IEnumerator Spawn_Customer(int customerAmount)
    {
        for (int i = 0; i < customerAmount; i++)
        {
            yield return new WaitForSeconds(spawnIntervalTime);

            Vector2 spawnPos = _gameController.dataController.Get_BoxArea_Position(0);
            Instantiate(_gameController.dataController.prefabs[0], spawnPos, Quaternion.identity);
        }
    }
}