using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Animation : MonoBehaviour
{
    private Animator _anim;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Animator anim)) { _anim = anim; }
    }

    // Custom
    public void Spawn_Effect()
    {
        LeanTween.alpha(gameObject, 0f, 0f);
        LeanTween.alpha(gameObject, 1f, 2f);
    }
}