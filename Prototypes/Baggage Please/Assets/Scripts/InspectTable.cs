using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectTable : MonoBehaviour
{
    private Game_Controller _gameController;
    public Game_Controller gameController => _gameController;

    [SerializeField] private Baggage_CheckPoint _acceptPoint;
    [SerializeField] private Baggage_CheckPoint _denyPoint;



    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    private void Start()
    {
        _acceptPoint.SetBaggage_Event += Accecpt_Baggage;
        _denyPoint.SetBaggage_Event += Deny_Baggage;
    }



    //
    private void Accecpt_Baggage()
    {
        for (int i = 0; i < _acceptPoint.currentBaggages.Count; i++)
        {
            _acceptPoint.currentBaggages[i].ownerNPC.interaction.Collect_Baggage();
            _acceptPoint.Remove_Baggage(_acceptPoint.currentBaggages[i]);
        }
    }

    private void Deny_Baggage()
    {

    }
}
