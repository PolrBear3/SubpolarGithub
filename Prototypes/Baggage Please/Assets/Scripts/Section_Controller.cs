using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section_Controller : MonoBehaviour
{
    private List<NPC_Controller> _currentNPCs = new();
    public List<NPC_Controller> currentNPCs => _currentNPCs;

    [SerializeField] private Transform _waitPoint;
    public Transform waitPoint => _waitPoint;

    // All NPCs Tracking
    public void Track_NPC(NPC_Controller npc)
    {
        _currentNPCs.Add(npc);
    }

    public void UnTrack_NPC(NPC_Controller npc)
    {
        _currentNPCs.Remove(npc);
    }

    //
    public void Line_NPCs()
    {
        for (int i = 0; i < _currentNPCs.Count; i++)
        {
            Vector2 setPoint = new(_waitPoint.transform.position.x - 0.5f * i, transform.position.y);
            _currentNPCs[i].movement.Set_TargetPoint(setPoint);
        }
    }
}