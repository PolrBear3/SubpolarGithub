using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_PositionClaimer : MonoBehaviour, ISaveLoadable
{
    private Main_Controller _main;

    [SerializeField] private List<Vector2> _claimPositions = new();


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();

        Claim_CustomPositions();
    }

    private void OnDestroy()
    {
        _main.UnClaim_Position(transform.position);
    }


    // ISaveLoadable
    public void Save_Data()
    {
        _main.UnClaim_Position(transform.position);
    }

    public void Load_Data()
    {
        
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
