using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game_Controller : MonoBehaviour
{
    private Data_Controller _data;
    public Data_Controller data => _data;

    private List<NPC_Controller> _allCurrentNPCs = new();
    public List<NPC_Controller> allCurrentNPCs => _allCurrentNPCs;

    [SerializeField] private List<Section_Controller> _sections = new();
    public List<Section_Controller> sections => _sections;

    [SerializeField] private List<Baggage_CheckPoint> _checkPoints = new();
    public List<Baggage_CheckPoint> checkPoints => _checkPoints;

    [Header("")]
    [SerializeField] private Transform _spawnPoint;
    public Transform spawnPoint => _spawnPoint;

    [SerializeField] private Vector2 _intervalTimeRange;



    // UnityEngine
    private void Awake()
    {
        _data = gameObject.GetComponent<Data_Controller>();
    }

    private void Start()
    {
        SetAll_SectionNum();
        SetAll_CheckPoints();

        Spawn_NPCs(5);
    }



    // Section Control
    private void SetAll_SectionNum()
    {
        for (int i = 0; i < _sections.Count; i++)
        {
            _sections[i].Set_SectionNum(i);
        }
    }

    private void SetAll_CheckPoints()
    {
        for (int i = 0; i < _checkPoints.Count; i++)
        {
            _checkPoints[i].Set_CheckPointNum(i);
        }
    }



    // NPC Control
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

            npc.Update_CurrentSection(_sections[0]);

            GameObject baggagePrefab = Instantiate(_data.baggagePrefab, Vector2.zero, Quaternion.identity);
            Baggage setBaggage = baggagePrefab.GetComponent<Baggage>();

            npc.interaction.Set_Baggage(setBaggage);
            setBaggage.Set_OwnerNPC(npc);

            Track_NPC(npc);
            _sections[0].Track_NPC(npc);

            _sections[0].Line_NPCs();

            float timeRange = Random.Range(_intervalTimeRange.x, _intervalTimeRange.y);
            yield return new WaitForSeconds(timeRange);
        }
    }



    // Tracking
    private void Track_NPC(NPC_Controller npc)
    {
        _allCurrentNPCs.Add(npc);
    }

    public void UnTrack_NPC(NPC_Controller npc)
    {
        _allCurrentNPCs.Remove(npc);
    }
}
