using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapPoint : MonoBehaviour
{
    SpriteRenderer sr;
    public bool objectPlaced;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Object_Placed_Check()
    {
        if (transform.childCount > 0)
        {
            objectPlaced = true;
            sr.enabled = false;
        }
        else 
        {
            objectPlaced = false;
            sr.enabled = true;
        }
    }

    public void Sprite_On()
    {
        sr.enabled = true;
    }

    public void Sprite_Off()
    {
        sr.enabled = false;
    }
}
