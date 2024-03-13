using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoinLauncher : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _coin;

    [Header("Data")]
    [SerializeField] private float _range;
    public float range => _range;

    [Header("Parabola Data")]
    [SerializeField] private float _angle;
    public float angle => _angle;

    [SerializeField] private float _gravity;
    public float gravity => _gravity;



    // Coin Launch
    public void Parabola_CoinLaunch(Coin_ScrObj coinType, Vector2 launchDirection)
    {
        GameObject launchedBullet = Instantiate(_coin, transform.parent.position, Quaternion.identity);
        CoinLauncher_Coin bullet = launchedBullet.GetComponent<CoinLauncher_Coin>();

        bullet.Set_Data(this, coinType, launchDirection);
        bullet.movementEvent += bullet.Parabola_Movement;
    }
}