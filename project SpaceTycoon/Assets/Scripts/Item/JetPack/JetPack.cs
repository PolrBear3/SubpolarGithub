using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : MonoBehaviour
{
    private void Update()
    {
        Press_Animation();
    }

    public Animator anim;

    public float flyForce;

    void Press_Animation()
    {
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("isPressed", true);
        }
        else
        {
            anim.SetBool("isPressed", false);
        }
    }

    public void Fuel()
    {

    }
}
