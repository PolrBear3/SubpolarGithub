using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_Object_Spawner : MonoBehaviour
{
    private void Update()
    {
        SpaceKey_Input();
    }

    public GameObject[] objects = new GameObject[0];
    public Transform[] positions = new Transform[0];

    int randomObjectNum;
    int randomPositionNum;

    void SpaceKey_Input()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Spawn_Random_Object();
        }
    }

    public void Spawn_Random_Object()
    {
        randomObjectNum = Random.Range(0, objects.Length);
        randomPositionNum = Random.Range(0, positions.Length);

        Instantiate(objects[randomObjectNum], positions[randomPositionNum]);
    }
}
