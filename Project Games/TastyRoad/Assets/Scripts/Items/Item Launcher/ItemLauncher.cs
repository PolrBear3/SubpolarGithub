using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemLauncher : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GameObject _foodBullet;
    [SerializeField] private GameObject _coinBullet;

    [Header("Data")]
    [SerializeField] private float _range;
    public float range => _range;

    [Header("Parabola Data")]
    [SerializeField] private float _angle;
    public float angle => _angle;

    [SerializeField] private float _gravity;
    public float gravity => _gravity;



    // Food Launch
    public void Parabola_FoodLaunch(Food_ScrObj food, Vector2 launchDirection)
    {
        GameObject launchedBullet = Instantiate(_foodBullet, transform.parent.position, Quaternion.identity);
        Food_Bullet bullet = launchedBullet.GetComponent<Food_Bullet>();

        bullet.Set_Food(food);

        bullet.Set_ItemLauncher(this);
        bullet.Set_Direction(launchDirection);

        bullet.movementEvent += bullet.Parabola_Movement;
    }



    // Coin Launch
    public void Parabola_CoinLaunch(Vector2 launchDirection)
    {
        GameObject launchedBullet = Instantiate(_coinBullet, transform.parent.position, Quaternion.identity);
        ItemLauncher_Bullet bullet = launchedBullet.GetComponent<ItemLauncher_Bullet>();

        bullet.Set_ItemLauncher(this);
        bullet.Set_Direction(launchDirection);

        bullet.movementEvent += bullet.Parabola_Movement;
    }
}