using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private NPC_Controller _controller;
    public NPC_Controller controller => _controller;


    [Header("")]
    [SerializeField] private CraftNPC[] _allCraftNPC;

    private CraftNPC _currentCraftNPC;
    public CraftNPC currentCraftNPC => _currentCraftNPC;


    [Header("")]
    [SerializeField] private AmountBar _nuggetBar;
    public AmountBar nuggetBar => _nuggetBar;

    [SerializeField] private AmountBar _timeBar;
    public AmountBar timeBar => _timeBar;


    private FoodData_Controller _foodIcon;
    public FoodData_Controller foodIcon => _foodIcon;

    private StationData _stationData;
    public StationData stationData => _stationData;


    // MonoBehaviour
    private void Start()
    {
        Set_CraftNPC();
    }


    //
    private void Set_CraftNPC(CraftNPC setNPC)
    {
        _currentCraftNPC = setNPC;
        _currentCraftNPC.SetInstance_CurrentNPC();
    }

    private void Set_CraftNPC()
    {
        int randIndex = Random.Range(0, _allCraftNPC.Length);
        Set_CraftNPC(_allCraftNPC[randIndex]);
    }
}
