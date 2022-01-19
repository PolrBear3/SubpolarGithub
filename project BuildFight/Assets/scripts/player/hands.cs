using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hands : MonoBehaviour
{
    BoxCollider2D bc;
    public static bool holding = false;

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
    }
    
    void Update()
    {
        if (holding == true)
        {
            bc.enabled = false;
        }
        else if (holding == false)
        {
            bc.enabled = true;
        }
    }
}
