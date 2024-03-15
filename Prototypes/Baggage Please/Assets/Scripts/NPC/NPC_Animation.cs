using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Animation : MonoBehaviour
{
    private Animator _anim;

    private NPC_Controller _controller;

    // UnityEngine
    private void Awake()
    {
        _anim = gameObject.GetComponent<Animator>();
        _controller = gameObject.GetComponent<NPC_Controller>();
    }

    //
}
