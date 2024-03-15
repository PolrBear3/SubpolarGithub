using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    private Data_Controller _data;

    private List<GameObject> _currentNPCs = new();
    public List<GameObject> currentNPCs => _currentNPCs;

    [SerializeField] private List<Section_Controller> _sections = new();
    public List<Section_Controller> sections => _sections;

    [Header("")]
    [SerializeField] private Vector2 _intervalTimeRange;

    // UnityEngine
    private void Awake()
    {
        _data = gameObject.GetComponent<Data_Controller>();
    }

    private void Start()
    {
        Spawn_NPCs(5);
    }

    //
    private void Spawn_NPCs(int amount)
    {
        StartCoroutine(Spawn_NPCs_Coroutine(amount));
    }
    private IEnumerator Spawn_NPCs_Coroutine(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject spawnNPC = Instantiate(_data.npcPrefab, _sections[0].waitPoint.position, Quaternion.identity);
            _currentNPCs.Add(spawnNPC);

            float timeRange = Random.Range(_intervalTimeRange.x, _intervalTimeRange.y);

            yield return new WaitForSeconds(timeRange);
        }
    }
}
