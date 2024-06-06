using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSign : ActionBubble_Interactable
{
    [Header("")]
    [SerializeField] private SubLocation _subLocation;


    // MonoBehaviour
    private new void Start()
    {
        base.Start();

        Action1 += _subLocation.Exit;
    }

    private void OnDestroy()
    {
        Action1 -= _subLocation.Exit;
    }
}
