using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    private Game_Controller _gameController;
    public Game_Controller gameController => _gameController;

    private NPC_Interaction _interaction;
    public NPC_Interaction interaction => _interaction;

    private NPC_Animation _animationControl;
    public NPC_Animation animationControl => _animationControl;

    private NPC_Movement _movement;
    public NPC_Movement movement => _movement;

    private Section_Controller _currentSection;
    public Section_Controller currentSection => _currentSection;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();

        _interaction = gameObject.GetComponent<NPC_Interaction>();
        _animationControl = gameObject.GetComponent<NPC_Animation>();
        _movement = gameObject.GetComponent<NPC_Movement>();
    }

    //
    public void Update_CurrentSection(Section_Controller section)
    {
        _currentSection = section;
    }
}
