using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looper : MonoBehaviour
{
    public GameObject[] objects = new GameObject[0];
    
    public void ForLooper()
    {
        for (int i=0; i < objects.Length; i++)
        {
            if (objects[i].activeSelf)
            {
                objects[i].SetActive(false);
            }
            else
            {
                objects[i].SetActive(true);
            }
        }
    }
    public void ForeachLooper()
    {
        foreach (GameObject gameobject in objects)
        {
            if (gameobject.activeSelf)
            {
                gameobject.SetActive(false);
            }
            else
            {
                gameobject.SetActive(true);
            }
        }
    }
    public void WhileLooper()
    {
        int i = 0;
        while (i < objects.Length)
        {
            if (objects[i].activeSelf)
            {
                objects[i].SetActive(false);
            }
            else
            {
                objects[i].SetActive(true);
            }

            i += 1;
        }
    }
}
