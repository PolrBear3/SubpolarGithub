using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

public class Location_Controller : MonoBehaviour
{
    [HideInInspector] public Main_Controller _mainController;

    [SerializeField] private List<SpriteRenderer> _roamAreas = new();

    [MinMaxSlider(1f, 60f)]
    [SerializeField] private Vector2 _spawnIntervalTime;

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

        Spawn_NPCs(5);
    }

    // Get
    public Vector2 Random_RoamArea(int arrayNum)
    {
        SpriteRenderer roamArea = _roamAreas[arrayNum];
        Vector2 centerPosition = roamArea.bounds.center;

        float randX = Random.Range(centerPosition.x - roamArea.bounds.extents.x, centerPosition.x + roamArea.bounds.extents.x);
        float randY = Random.Range(centerPosition.y - roamArea.bounds.extents.y, centerPosition.y + roamArea.bounds.extents.y);

        return new Vector2(randX, randY);
    }

    // Roam Areas Control
    private void RoamAreas_ColorToggle(bool toggleOn)
    {
        if (toggleOn == true) return;

        for (int i = 0; i < _roamAreas.Count; i++)
        {
            _roamAreas[i].color = Color.clear;
        }
    }

    // NPC Spawn
    private IEnumerator Spawn_NPCs_Coroutine(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            _mainController.Spawn_Character(1, _mainController.OuterCamera_Position(-1, 0, -2, -3));

            float randIntervalTime = Random.Range(_spawnIntervalTime.x, _spawnIntervalTime.y);
            yield return new WaitForSeconds(randIntervalTime);
        }
    }
    private void Spawn_NPCs(int amount)
    {
        StartCoroutine(Spawn_NPCs_Coroutine(amount));
    }
}