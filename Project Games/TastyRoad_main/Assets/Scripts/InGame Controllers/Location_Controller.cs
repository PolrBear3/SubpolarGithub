using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location_Controller : MonoBehaviour
{
    [HideInInspector] public Main_Controller _mainController;

    public List<SpriteRenderer> roamAreas = new();

    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentLocaiton(this);
    }

    private void Start()
    {
        // Toggle Off All Roam Area Colors On Game Start
        RoamAreas_ColorToggle(false);

        // NPC Spawn Test
        Spawn_NPCs(1, 2);
    }

    // Get
    public Vector2 Random_RoamArea(int arrayNum)
    {
        SpriteRenderer roamArea = roamAreas[arrayNum];
        Vector2 centerPosition = roamArea.bounds.center;

        float randX = Random.Range(centerPosition.x - roamArea.bounds.extents.x, centerPosition.x + roamArea.bounds.extents.x);
        float randY = Random.Range(centerPosition.y - roamArea.bounds.extents.y, centerPosition.y + roamArea.bounds.extents.y);

        return new Vector2(randX, randY);
    }

    // Roam Areas Control
    private void RoamAreas_ColorToggle(bool toggleOn)
    {
        if (toggleOn == true) return;

        for (int i = 0; i < roamAreas.Count; i++)
        {
            roamAreas[i].color = Color.clear;
        }
    }

    // NPC Spawn Test
    private IEnumerator Spawn_NPCs_Coroutine(int amount, float intervalTime)
    {
        for (int i = 0; i < amount; i++)
        {
            _mainController.Spawn_Character(1, _mainController.OuterCamera_Position(-1, 0, -3, -3));

            yield return new WaitForSeconds(intervalTime);
        }
    }
    private void Spawn_NPCs(int amount, float intervalTime)
    {
        StartCoroutine(Spawn_NPCs_Coroutine(amount, intervalTime));
    }
}