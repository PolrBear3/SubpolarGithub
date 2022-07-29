using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Controller : MonoBehaviour
{
    public GameObject box;
    public Transform instantiateLocation;

    public void Instantiate_Box()
    {
        Instantiate(box, instantiateLocation);
    }
}
