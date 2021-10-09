using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    public GameObject theFalsePresence;
    float location;
    Vector2 wheretoSpawn;
    public float spawnRate;
    float nextSpawn = 0f;

    void Update()
    {
        if(Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;
            location = Random.Range(-16.3848f, -16.3849f);
            wheretoSpawn = new Vector2(location, transform.position.y);
            Instantiate(theFalsePresence, wheretoSpawn, Quaternion.identity);
        }
    }
}
