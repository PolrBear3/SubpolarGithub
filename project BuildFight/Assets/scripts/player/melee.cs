using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class melee : MonoBehaviour
{
    Animator anim;
    
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("melee");
        }
    }
}
