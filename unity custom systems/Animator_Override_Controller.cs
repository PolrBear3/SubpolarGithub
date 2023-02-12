using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator_Override_Controller : MonoBehaviour
{
    public Animator anim;
    public AnimatorOverrideController controllerA;
    public AnimatorOverrideController controllerB;

    public void Change_Animator_to_A()
    {
        anim.runtimeAnimatorController = controllerA;
    }
    
    public void Change_Animator_to_B()
    {
        anim.runtimeAnimatorController = controllerB;
    }
}