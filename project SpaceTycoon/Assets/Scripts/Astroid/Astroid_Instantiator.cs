using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid_Instantiator : SpaceTycoon_Main_GameController
{
    public GameObject astroid;
    float randY;
    Vector2 whereToSpawn;
    public float spawnCoolTime = 1f, coolTimeChangeValue = 0.1f;
    float currentRate;
    int currentEnginesOn = 0;

    private void Update()
    {
        Instantiate_Astroid();
        SpawnCoolTime_Conditions();
    }

    void SpawnCoolTime_Conditions()
    {
        if (currentEnginesOn < EnginesOn)
        {
            spawnCoolTime -= coolTimeChangeValue;

            currentEnginesOn += 1;
        }
        if (currentEnginesOn > EnginesOn)
        {
            spawnCoolTime += coolTimeChangeValue;

            currentEnginesOn -= 1;
        }
    }

    void Instantiate_Astroid()
    {
        if (EnginesOn > 0)
        {
            currentRate += Time.deltaTime;

            if (currentRate >= spawnCoolTime)
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
