using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineDetector : MonoBehaviour
{
    private Game_Controller _gameController;
    public Game_Controller gameController => _gameController;

    [SerializeField] private Transform _baggagePoint;
    [SerializeField] private Transform _hidePoint;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }

    private void Start()
    {
        GetNPC_Baggage();
    }

    private void Update()
    {
        
    }

    //
    private void GetNPC_Baggage()
    {
        StartCoroutine(GetNPC_Baggage_Coroutine());
    }
    private IEnumerator GetNPC_Baggage_Coroutine()
    {
        while (true)
        {
            Section_Controller section1 = _gameController.sections[0];

            while (section1.currentNPCs.Count <= 0) yield return null;

            NPC_Controller closestNPC = section1.currentNPCs[0];

            if (section1.At_WaitPoint(closestNPC.transform))
            {
                if (closestNPC.interaction.baggage.baggageLocation != _baggagePoint)
                {
                    closestNPC.interaction.Drop_Baggage(_baggagePoint);

                    Baggage npcBag = closestNPC.interaction.baggage;

                    npcBag.Set_Location(_baggagePoint);
                    npcBag.Droppable_Toggle(true);
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void Move_DropPoint_Baggages()
    {

    }
}