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
        Spawn_Customer(_gameController.dataController.Get_OuterCamera_Position(-1, 0, -3, -3), 5, startIntervalTime);
    }

    // Custom
    private IEnumerator Spawn_Customer_Delay(Vector2 area, int customerAmount, float intervalTime)
    {
        for (int i = 0; i < customerAmount; i++)
        {
            yield return new WaitForSeconds(intervalTime);
            Instantiate(_gameController.dataController.characters[0], area, Quaternion.identity);
        }
    }
    public void Spawn_Customer(Vector2 area, int customerAmount, float intervalTime)
    {
        StartCoroutine(Spawn_Customer_Delay(area, customerAmount, intervalTime));
    }
}