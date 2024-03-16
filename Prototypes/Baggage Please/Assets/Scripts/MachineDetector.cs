using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineDetector : MonoBehaviour
{
    private Game_Controller _gameController;
    public Game_Controller gameController => _gameController;

    [SerializeField] private Baggage_HidePoint _dropPoint;
    [SerializeField] private Baggage_DropPoint _movePoint;
    [SerializeField] private Baggage_HidePoint _endPoint;

    private InspectTable _inspectTable;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
        _inspectTable = FindObjectOfType<InspectTable>();
    }

    private void Start()
    {
        Collect_Baggage();
    }

    // Section 1
    private void Collect_Baggage()
    {
        StartCoroutine(Collect_Baggage_Coroutine());
    }
    private IEnumerator Collect_Baggage_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (_gameController.sections[0].currentNPCs.Count <= 0) continue;

            NPC_Controller closestNPC = _gameController.sections[0].currentNPCs[0];
            Baggage NPCbaggage = closestNPC.interaction.baggage;

            if (NPCbaggage.dragging) continue;
            if (_gameController.sections[0].At_WaitPoint(closestNPC.transform) == false) continue;

            NPCbaggage.Droppable_Toggle(true);

            if (NPCbaggage.droppable == false) continue;
            if (closestNPC.interaction.hasBaggage == false) continue;

            NPCbaggage.Set_DropPoint(_dropPoint.baggagePosition);
            closestNPC.interaction.HasBaggage_Update();
        }
    }

    // Move Bags
    private void Move_Baggages()
    {

    }
}