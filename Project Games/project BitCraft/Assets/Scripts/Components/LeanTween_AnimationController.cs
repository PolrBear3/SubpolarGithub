using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTween_AnimationController : MonoBehaviour
{
    public void Interact_Animation()
    {
        float originalPos = transform.localPosition.x;

        // left
        float leftPos = originalPos - .04f;
        LeanTween.moveLocalX(gameObject, leftPos, .25f);

        // return
        LeanTween.moveLocalX(gameObject, originalPos, .25f).setDelay(.25f);
    }
    public void Damage_Animation()
    {

    }
}
