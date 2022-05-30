using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapPoint : MonoBehaviour
{
    public bool objectPlaced = false;

    public void Object_Placed_Check()
    {
        if (transform.childCount > 0)
        {
            objectPlaced = true;
        }
    }
}
