using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_PositionClaimer : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Vector2[] _surroundPositions;
    [SerializeField] private Vector2[] _interactPositions;

    [SerializeField] private List<Vector2> _claimPositions = new();

    [Header("")]
    [SerializeField] private bool _unClaimOnStart;


    // UnityEngine
    private void Awake()
    {
        if (_unClaimOnStart) return;

        Claim_CurrentPositions();
    }

    private void OnDestroy()
    {
        UnClaim_CurrentPositions();
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

    /// <summary>
    /// Claimed positions included
    /// </summary>
    public List<Vector2> All_InteractPositions()
    {
        Vector2 currentPosition = transform.position;
        List<Vector2> allPositions = new();

        foreach (Vector2 position in _interactPositions)
        {
            allPositions.Add(currentPosition + position);
        }

        return allPositions;
    }

    /// <summary>
    /// Claimed positions excluded
    /// </summary>
    public List<Vector2> All_SurroundPositions()
    {
        Vector2 currentPosition = transform.position;
        List<Vector2> surroundPositions = new();

        foreach (Vector2 position in _surroundPositions)
        {
            surroundPositions.Add(currentPosition + position);
        }

        return surroundPositions;
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

    public bool CurrentPositions_Claimed()
    {
        Main_Controller main = Main_Controller.instance;

        foreach (Vector2 position in Current_Positions())
        {
            Vector2 snapPos = main.SnapPosition(position);
            if (main.Position_Claimed(snapPos) == false) continue;

            return true;
        }
        return false;
    }


    public Vector2 Claim_CurrentPositions()
    {
        Main_Controller main = Main_Controller.instance;

        List<Vector2> snapPositions = new();

        foreach (Vector2 position in Current_Positions())
        {
            Vector2 snapPos = main.SnapPosition(position);
            snapPositions.Add(snapPos);

            main.Claim_Position(snapPos);
        }

        return snapPositions[snapPositions.Count / 2];
    }

    public void UnClaim_CurrentPositions()
    {
        Main_Controller main = Main_Controller.instance;

        foreach (Vector2 position in Current_Positions())
        {
            Vector2 snapPos = main.SnapPosition(position);
            main.UnClaim_Position(snapPos);
        }
    }
}
