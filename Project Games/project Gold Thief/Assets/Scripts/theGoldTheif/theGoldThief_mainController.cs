using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class theGoldThief_mainController_script
{
    public theGoldThief_movement movement;
}

[System.Serializable]
public class theGoldThief_mainController_connection
{
    public Transform mainTransform;
    public SpriteRenderer mainSR;
    public BoxCollider2D mainBC;
    public Rigidbody2D mainRB;
    public Animator mainAnim;
}

public class theGoldThief_mainController : MonoBehaviour
{
    public theGoldThief_mainController_script script;
    public theGoldThief_mainController_connection connection;
}
