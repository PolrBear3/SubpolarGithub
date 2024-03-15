using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    private NPC_Movement _movement;
    public NPC_Movement movement => _movement;

    private int _sectionNum;
    public int sectionNum => _sectionNum;

    // UnityEngine
    private void Awake()
    {
        _movement = gameObject.GetComponent<NPC_Movement>();
    }
}
