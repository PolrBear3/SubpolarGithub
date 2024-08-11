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


    // UnityEngine
    private void Start()
    {
        Audio_Controller.instance.Create_EventInstance("CoinLauncher_throw", gameObject);
    }

    private void OnDestroy()
    {
        Audio_Controller.instance.Remove_EventInstance(gameObject);
    }


    /// <returns>
    /// Launched coin
    /// </returns>
    public CoinLauncher_Coin Parabola_CoinLaunch(Sprite sprite, Vector2 launchDirection)
    {
        GameObject launchedBullet = Instantiate(_coin, transform.parent.position, Quaternion.identity);
        CoinLauncher_Coin bullet = launchedBullet.GetComponent<CoinLauncher_Coin>();

        Vector2 direction = launchDirection - (Vector2)transform.position;

        bullet.SetData(this, sprite, direction);
        bullet.movementEvent += bullet.Parabola_Movement;

        float pitchValue = Random.Range(0, 1f);
        Audio_Controller.instance.Set_EventInstance_Parameter(gameObject, "Value_intensity", pitchValue);
        Audio_Controller.instance.EventInstance(gameObject).start();

        return bullet;
    }
}