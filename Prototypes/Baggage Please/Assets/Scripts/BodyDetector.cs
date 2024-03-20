using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyDetector : MonoBehaviour
{
    private Game_Controller _gameController;



    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }



    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out NPC_Controller npc)) return;

        _gameController.checkPoints[npc.interaction.baggage.checkNum].Remove_Baggage(npc.interaction.baggage);
        npc.interaction.Collect_Baggage();

        Data_Controller.score += Calculated_BaggageScore(npc.interaction.baggage);
        _gameController.data.ScoreText_Update();

        Destroy(npc.gameObject, 3f);
    }



    //
    private int Calculated_BaggageScore(Baggage bag)
    {
        int bagHeatLevel = bag.data.heatLevel;
        int scoreCount = 0;

        // heat level check
        if (bagHeatLevel <= 0) scoreCount++;
        else scoreCount -= bagHeatLevel;

        // level 3 penalty
        if (bagHeatLevel >= 3) scoreCount -= bagHeatLevel;

        return scoreCount;
    }
}
