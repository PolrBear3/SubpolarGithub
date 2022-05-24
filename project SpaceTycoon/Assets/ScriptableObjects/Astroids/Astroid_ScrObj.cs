using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create New Astroid")]
public class Astroid_ScrObj : ScriptableObject
{
    public Sprite astroidSprite;
    public float accelerationRate = 0.05f;
    public float speedChangeValue = 0.5f;
}
