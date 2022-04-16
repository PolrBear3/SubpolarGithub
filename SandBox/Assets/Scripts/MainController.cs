using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    private void Update()
    {
        Debug.Log(redBox.boxDetected);
    }

    public Box redBox;
    public Box blueBox;
}
