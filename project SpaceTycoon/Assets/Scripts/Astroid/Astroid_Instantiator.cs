using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid_Instantiator : SpaceTycoon_Main_GameController
{
    public GameObject astroid;
    float randY;
    Vector2 whereToSpawn;
    public float spawnRate;
    float currentRate;

    private void Update()
    {
        Instantiate_Astroid();
    }

    void Instantiate_Astroid()
    {
        if (EnginesOn > 0)
        {
            currentRate += Time.deltaTime;

            if (currentRate >= spawnRate)
            {
                randY = Random.Range(-3f, 3f);
                whereToSpawn = new Vector2(transform.position.x, randY);
                var spawnedAstroid = Instantiate(astroid, whereToSpawn, Quaternion.identity);

                // place spawned astroid as a child inside parent instantiator 
                spawnedAstroid.transform.parent = gameObject.transform;

                // reset repeat timer
                currentRate = 0;
            }
        }
    }
}
