using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLauncher : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;



    //
    public void Launch()
    {

    }

    public void Parabola_Launch(float launchDirection)
    {
        GameObject launchBullet = Instantiate(_bullet, transform);

        ItemLauncher_Bullet bullet = launchBullet.GetComponent<ItemLauncher_Bullet>();

        bullet.Set_Direction(launchDirection);
        bullet.Movement += bullet.Parabola_Movement;
    }
}