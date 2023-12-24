using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Controller : MonoBehaviour
{
    private Game_Controller _gameController;

    [Header("Data")]
    public float startIntervalTime;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Game_Controller gameController)) { _gameController = gameController; }
    }
    private void Start()
    {
        Spawn_Customer(5, startIntervalTime);
    }

    // Custom
    private IEnumerator Spawn_Customer_Delay(int customerAmount, float intervalTime)
    {
        for (int i = 0; i < customerAmount; i++)
        {
            yield return new WaitForSeconds(intervalTime);

            Vector2 spawnPos = _gameController.dataController.Get_BoxArea_Position(0);
            Instantiate(_gameController.dataController.prefabs[0], spawnPos, Quaternion.identity);
        }
    }
    public void Spawn_Customer(int customerAmount, float intervalTime)
    {
        StartCoroutine(Spawn_Customer_Delay(customerAmount, intervalTime));
    }
}