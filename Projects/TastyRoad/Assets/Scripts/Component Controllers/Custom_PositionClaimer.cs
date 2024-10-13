using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_PositionClaimer : MonoBehaviour, ISaveLoadable
{
    private Main_Controller _main;

    [Header("")]
    [SerializeField] private List<Vector2> _claimPositions = new();


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();

        Claim_CustomPositions();
    }

    private void OnDestroy()
    {
        UnClaim_CustomPositions();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        UnClaim_CustomPositions();
    }

    public void Load_Data()
    {

    }


    //
    private List<Vector2> Current_Positions()
    {
        List<Vector2> currentPositions = new();

        foreach (Vector2 position in _claimPositions)
        {
            float xPos = transform.position.x + position.x;
            float yPos = transform.position.y + position.y;

            currentPositions.Add(new(xPos, yPos));
        }

        return currentPositions;
    }


    private void Claim_CustomPositions()
    {
        foreach (Vector2 position in Current_Positions())
        {
            Vector2 snapPos = Main_Controller.SnapPosition(position);
            _main.Claim_Position(snapPos);
        }
    }

    private void UnClaim_CustomPositions()
    {
        foreach (Vector2 position in Current_Positions())
        {
            Vector2 snapPos = Main_Controller.SnapPosition(position);
            _main.UnClaim_Position(snapPos);
        }
    }
}
