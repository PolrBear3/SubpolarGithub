using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float dropRate = 2f;
    float nextDropTime = 0f;

    private void Update()
    {
        if (Time.time > nextDropTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Drop();
                nextDropTime = Time.time + 1f / dropRate;
            }
        }
    }

    public Transform dropPoint;
    public GameObject present;
    void Drop()
    {
        Instantiate(present, dropPoint.position, dropPoint.rotation);
    }
}
