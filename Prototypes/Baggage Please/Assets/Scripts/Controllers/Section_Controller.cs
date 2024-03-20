using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section_Controller : MonoBehaviour
{
    private List<NPC_Controller> _currentNPCs = new();
    public List<NPC_Controller> currentNPCs => _currentNPCs;

    [Header("")]
    [SerializeField] private Transform _waitPoint;
    public Transform waitPoint => _waitPoint;

    [Header("")]
    [SerializeField] private float _seperateDistance;

    private int _sectionNum;
    public int sectionNum => _sectionNum;

    //
    public void Set_SectionNum(int num)
    {
        _sectionNum = num;
    }

    // Checks
    public bool At_WaitPoint(Transform transform)
    {
        float distance = Vector2.Distance(_waitPoint.position, transform.position);

        if (distance < 0.1f) return true;
        else return false;
    }

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
        // refresh
        for (int i = _currentNPCs.Count - 1; i >= 0; i--)
        {
            if (_currentNPCs[i] == null) _currentNPCs.RemoveAt(i);
        }

        for (int i = 0; i < _currentNPCs.Count; i++)
        {
            Vector2 setPoint = new(_waitPoint.transform.position.x - _seperateDistance * 0.1f * i, transform.position.y);
            _currentNPCs[i].movement.Set_TargetPoint(setPoint);
        }
    }
}