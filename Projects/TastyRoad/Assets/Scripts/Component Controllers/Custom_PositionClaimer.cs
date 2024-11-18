using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_PositionClaimer : MonoBehaviour
{
    private Main_Controller _main;

    [Header("")]
    [SerializeField] private Vector2[] _allPositions;
    [SerializeField] private List<Vector2> _claimPositions = new();


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();

        Claim_CurrentPositions();
    }

    private void OnDestroy()
    {
        UnClaim_CurrentPositions();
    }


    //
    public List<Vector2> All_Positions()
    {
        Vector2 currentPosition = transform.position;
        List<Vector2> allPositions = new();

        foreach (Vector2 position in _allPositions)
        {
            allPositions.Add(currentPosition + position);
        }

        return allPositions;
    }

    public List<Vector2> Current_Positions()
    {
        Vector2 currentPosition = transform.position;
        List<Vector2> currentPositions = new();

        foreach (Vector2 position in _claimPositions)
        {
            currentPositions.Add(currentPosition + position);
        }

        return currentPositions;
    }


    public bool Is_ClaimPosition(Vector2 checkPosition)
    {
        for (int i = 0; i < Current_Positions().Count; i++)
        {
            if (Current_Positions()[i] != checkPosition) continue;
            return true;
        }
        return false;
    }


    public void Claim_CurrentPositions()
    {
        foreach (Vector2 position in Current_Positions())
        {
            Vector2 snapPos = Main_Controller.SnapPosition(position);
            _main.Claim_Position(snapPos);
        }
    }

    public void UnClaim_CurrentPositions()
    {
        foreach (Vector2 position in Current_Positions())
        {
            Vector2 snapPos = Main_Controller.SnapPosition(position);
            _main.UnClaim_Position(snapPos);
        }
    }
}
