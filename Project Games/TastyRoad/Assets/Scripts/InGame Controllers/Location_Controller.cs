using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location_Controller : MonoBehaviour
{
    [HideInInspector] public Main_Controller _mainController;

    [SerializeField] private SpriteRenderer _roamArea;
    public SpriteRenderer roamArea => _roamArea;

    [Header("")]
    [SerializeField] private Vector2 _spawnIntervalTime;

    // add breakfast lunch dinner max spawn amount
    [SerializeField] private int _maxSpawn;



    // UnityEngine
    private void Awake()
    {
        _mainController = FindObjectOfType<Main_Controller>();
        _mainController.Track_CurrentLocaiton(this);
    }

    private void Start()
    {
        // Toggle Off All Roam Area Colors On Game Start
        _roamArea.color = Color.clear;

        NPC_Spawn_Control();
    }



    /// <summary>
    /// Update function for npc spawn and amount control
    /// </summary>
    public void NPC_Spawn_Control()
    {
        StartCoroutine(NPC_Spawn_Control_Coroutine());
    }
    private IEnumerator NPC_Spawn_Control_Coroutine()
    {
        while (true)
        {
            while (_mainController.currentCharacters.Count >= _maxSpawn) yield return null;

            float randIntervalTime = Random.Range(_spawnIntervalTime.x, _spawnIntervalTime.y);
            yield return new WaitForSeconds(randIntervalTime);

            _mainController.Spawn_Character(1, _mainController.OuterCamera_Position(Random.Range(0, 2)));
        }
    }
}