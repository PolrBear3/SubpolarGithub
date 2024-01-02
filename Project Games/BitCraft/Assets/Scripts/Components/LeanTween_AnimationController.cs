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
        LeanTween.moveLocalX(gameObject, leftPos, .15f);

        // return
        LeanTween.moveLocalX(gameObject, originalPos, .15f).setDelay(.15f);
    }
    public void Damage_Animation()
    {

    }
}
