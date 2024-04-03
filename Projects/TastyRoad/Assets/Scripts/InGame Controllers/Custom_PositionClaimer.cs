using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_PositionClaimer : MonoBehaviour
{
    private Main_Controller _main;

    [SerializeField] private List<Vector2> _claimPositions = new();



    // UnityEngine
    private void Awake()
    {
        _main = FindObjectOfType<Main_Controller>();

        Claim_CustomPositions();
    }



    //
    private void Claim_CustomPositions()
    {
        for (int i = 0; i < _claimPositions.Count; i++)
        {
             _main.Claim_Position(new Vector2(transform.position.x + _claimPositions[i].x, transform.position.y + _claimPositions[i].y));
        }
    }
}
