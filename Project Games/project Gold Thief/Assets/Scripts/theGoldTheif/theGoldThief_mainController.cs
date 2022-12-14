using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class TheGoldThief_MainController_Script
{
    public TheGoldThief_Movement movement;
}

[System.Serializable]
public class TheGoldThief_MainController_Connection
{
    public LayerMask groundMask;
    public Transform mainTransform;
    public Transform footTransform;
    public SpriteRenderer mainSR;
    public BoxCollider2D mainBC;
    public Rigidbody2D mainRB;
    public Animator mainAnim;
}

public class TheGoldThief_MainController : MonoBehaviour
{
    public TheGoldThief_MainController_Script script;
    public TheGoldThief_MainController_Connection connection;
}
