using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationTile : MonoBehaviour
{
    private Image _image;

    private Animator _anim;
    public Animator anim => _anim;


    private bool _locked;
    public bool locked => _locked;


    // UnityEngine
    private void Awake()
    {
        _image = gameObject.GetComponent<Image>();
        _anim = gameObject.GetComponent<Animator>();
    }


    // Sets
    public Animator Set_AnimatorOverrider(AnimatorOverrideController overrider)
    {
        _anim.runtimeAnimatorController = overrider;
        return _anim;
    }


    // Controls
    public void Toggle_Lock(bool toggle)
    {
        _locked = toggle;

        if (toggle == false) return;

        _anim.enabled = false;
        _image.color = Color.black;
    }
}
