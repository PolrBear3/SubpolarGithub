using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Movement : MonoBehaviour
{
    private Animator _anim;

    [SerializeField] private float _moveSpeed;


    // UnityEngine
    private void Awake()
    {
        _anim = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        Moveto_Next();
    }

    //
    private void Moveto_Next()
    {
        transform.Translate(_moveSpeed * Time.deltaTime * new Vector2(1f, 0f));
    }
}
