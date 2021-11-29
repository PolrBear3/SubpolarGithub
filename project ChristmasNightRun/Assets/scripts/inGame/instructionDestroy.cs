using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class instructionDestroy : MonoBehaviour
{
    void Update()
    {
        Destroy(gameObject, 4f);
    }
}
