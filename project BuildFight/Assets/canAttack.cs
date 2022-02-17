using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canAttack : MonoBehaviour
{
    [HideInInspector]
    public bool enableAttack;

    void Start()
    {
        enableAttack = true;
    }
}
