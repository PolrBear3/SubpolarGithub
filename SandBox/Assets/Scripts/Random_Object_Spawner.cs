using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_Object_Spawner : MonoBehaviour
{
    public GameObject[] objects = new GameObject[0];
    public Transform position;
    GameObject randomObject;
    int randomObjectNum;

    public void Spawn_Random_Object()
    {
        randomObjectNum = Random.Range(0, objects.Length);
        randomObject = objects[randomObjectNum];
        Instantiate(randomObject, position);
    }
}
