using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    private Data_Controller _data;

    private List<NPC_Controller> _allCurrentNPCs = new();
    public List<NPC_Controller> allCurrentNPCs => _allCurrentNPCs;

    [SerializeField] private List<Section_Controller> _sections = new();
    public List<Section_Controller> sections => _sections;

    [Header("")]
    [SerializeField] private Transform _spawnPoint;
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
            GameObject spawnNPC = Instantiate(_data.npcPrefab, _spawnPoint.position, Quaternion.identity);
            NPC_Controller npc = spawnNPC.GetComponent<NPC_Controller>();

            Track_NPC(npc);
            _sections[npc.sectionNum].Track_NPC(npc);

            _sections[npc.sectionNum].Line_NPCs();

            float timeRange = Random.Range(_intervalTimeRange.x, _intervalTimeRange.y);
            yield return new WaitForSeconds(timeRange);
        }
    }

    // All NPCs Tracking
    private void Track_NPC(NPC_Controller npc)
    {
        _allCurrentNPCs.Add(npc);
    }

    public void UnTrack_NPC(NPC_Controller npc)
    {
        _allCurrentNPCs.Remove(npc);
    }
}
