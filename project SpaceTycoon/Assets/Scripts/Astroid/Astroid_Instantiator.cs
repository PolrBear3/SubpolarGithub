using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid_Instantiator : SpaceTycoon_Main_GameController
{
    public GameObject astroid;    
    float randY;
    float randY2;
    float randY3;
    Vector2 whereToSpawn;
    Vector2 whereToSpawn2;
    Vector2 whereToSpawn3;
    public float spawnRate;
    public float spawnRate2;
    public float spawnRate3;
    float nextSpawn = 0.0f;
    float nextSpawn2 = 0.0f;
    float nextSpawn3 = 0.0f;

    void Update()
    {
        Shoot_Sector1_Astroids();
    }

    void Shoot_Sector1_Astroids()
    {
        if (Time.time > nextSpawn && EnginesOn > 0)
        {
            nextSpawn = Time.time + spawnRate;
           
            randY = Random.Range(-1f, 1f);
            whereToSpawn = new Vector2 (transform.position.x, randY);
            Instantiate(astroid, whereToSpawn, Quaternion.identity);
        }
        if (Time.time > nextSpawn2 && EnginesOn > 0)
        {
            nextSpawn2 = Time.time + spawnRate2;

            randY2 = Random.Range(-2f, 2f);
            whereToSpawn2 = new Vector2(transform.position.x, randY2);
            Instantiate(astroid, whereToSpawn2, Quaternion.identity);
        }
        if (Time.time > nextSpawn3 && EnginesOn > 0)
        {
            nextSpawn3 = Time.time + spawnRate3;

            randY3 = Random.Range(-3f, 3f);
            whereToSpawn3 = new Vector2(transform.position.x, randY3);
            Instantiate(astroid, whereToSpawn3, Quaternion.identity);
        }
    }
}
