using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enumeration_System : MonoBehaviour
{
    public enum Movements { left, right, up, down }

    public Movements movement;

    public void Switch_Function()
    {
        switch (movement)
        {
            case Movements.left:
                Debug.Log("Player moving left");
                break;
            case Movements.right:
                Debug.Log("Player moving right");
                break;
            case Movements.up:
                Debug.Log("Player jumped");
                break;
            case Movements.down:
                Debug.Log("Player ducked");
                break;
        }
    }
}
