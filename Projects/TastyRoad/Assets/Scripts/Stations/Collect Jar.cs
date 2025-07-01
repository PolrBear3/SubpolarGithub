using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectJar : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Station_Controller _controller;

    [Space(20)]
    [SerializeField] private Sprite[] _jarSprites;

    [Space(20)]
    [SerializeField][Range(0, 100)] private int _searchTime;
    
    [Space(10)]
    [SerializeField][Range(0, 100)] private float _donateTime;
    [SerializeField] [Range(0, 100)] private int _donateGoldAmount;


    private List<NPC_Controller> _trackNPCs = new();
    
    private Coroutine _coroutine;
    private Coroutine _donateCoroutine;

    
    // MonoBehaviour
    private void Start()
    {
        Update_Sprite();
        Toggle_AmountBar();

        Collect(_controller.movement.enabled == false);
        Donate_Collect(_controller.movement.enabled == false);

        // subscriptions
        Main_Controller.instance.OnFoodBookmark += Collect;
        _controller.movement.OnLoadPosition += Collect;

        _controller.movement.OnLoadPosition += Donate_Collect;

        Detection_Controller detection = _controller.detection;

        detection.EnterEvent += Toggle_AmountBar;
        detection.ExitEvent += Toggle_AmountBar;

        IInteractable_Controller iInteractable = _controller.iInteractable;

        iInteractable.OnInteract += Toggle_GoldAmount;
        iInteractable.OnHoldInteract += Empty;
        
        _controller.OnStationDestroy += Empty;
        _controller.OnStationDestroy += Reset_TrackNPCs;
    }

    private void OnDestroy()
    {
        Empty();

        // subscriptions
        Main_Controller.instance.OnFoodBookmark -= Collect;
        _controller.movement.OnLoadPosition -= Collect;
        
        _controller.movement.OnLoadPosition -= Donate_Collect;

        Detection_Controller detection = _controller.detection;

        detection.EnterEvent -= Toggle_AmountBar;
        detection.ExitEvent -= Toggle_AmountBar;

        IInteractable_Controller iInteractable = _controller.iInteractable;

        iInteractable.OnInteract -= Toggle_GoldAmount;
        iInteractable.OnHoldInteract -= Empty;
        
        _controller.OnStationDestroy -= Empty;
        _controller.OnStationDestroy -= Reset_TrackNPCs;
    }


    // Indications
    private void Update_Sprite()
    {
        SpriteRenderer sr = _controller.spriteRenderer;

        if (Current_Amount() <= 0)
        {
            sr.sprite = _jarSprites[0];
            return;
        }

        sr.sprite = _jarSprites[1];
    }


    private void Toggle_GoldAmount()
    {
        if (Current_Amount() <= 0) return;

        Sprite jarSprite = _controller.stationScrObj.dialogIcon;
        int currentAmount = _controller.Food_Icon().currentData.currentAmount;

        GoldSystem.instance.Indicate_TriggerData(new(jarSprite, currentAmount));
    }

    private void Toggle_AmountBar()
    {
        FoodData_Controller foodIcon = _controller.Food_Icon();
        bool toggle = Current_Amount() > 0 && _controller.detection.player != null;

        foodIcon.Hide_Icon();
        foodIcon.Toggle_AmountBar(Current_Amount() > 0 && toggle);
    }


    // Main
    private int Current_Amount()
    {
        FoodData_Controller foodIcon = _controller.Food_Icon();
        if (foodIcon.hasFood == false) return 0;

        return foodIcon.currentData.currentAmount;
    }


    private void Insert(int insertAmount)
    {
        if (insertAmount <= 0) return;

        FoodData_Controller stationIcon = _controller.Food_Icon();
        if (stationIcon.Is_MaxAmount()) return;

        // set any food for only amount control
        Food_ScrObj randFood = Main_Controller.instance.dataController.RawFood();
        Food_ScrObj setFood = stationIcon.hasFood ? stationIcon.currentData.foodScrObj : randFood;

        stationIcon.Update_Amount(setFood, insertAmount);

        Update_Sprite();
        Toggle_AmountBar();

        // sfx
        Audio_Controller.instance.Play_OneShot(gameObject, 2);
    }

    private void Empty()
    {
        if (Current_Amount() <= 0) return;

        GoldSystem.instance.Update_CurrentAmount(Current_Amount());

        _controller.Food_Icon().Update_AllDatas(null);

        Update_Sprite();
        Toggle_AmountBar();
    }
    
    
    // NPC Tracking
    private void Track_NPC(NPC_Controller trackNPC)
    {
        if (_trackNPCs.Contains(trackNPC)) return;
        _trackNPCs.Add(trackNPC);
    }

    private void Refresh_TrackNPCs()
    {
        List<NPC_Controller> trackNPCs = new(_trackNPCs);

        for (int i = 0; i < trackNPCs.Count; i++)
        {
            NPC_Movement movement = trackNPCs[i].movement;
            bool incoming = movement.roamActive == false && movement.targetPosition == (Vector2)transform.position;

            if (incoming && movement.At_TargetPosition(transform.position) == false)
            {
                Track_NPC(trackNPCs[i]);
                continue;
            }
            _trackNPCs.Remove(trackNPCs[i]);
        }
    }

    private void Reset_TrackNPCs()
    {
        foreach (NPC_Controller npc in _trackNPCs)
        {
            npc.foodInteraction.Update_RoamArea();
        }
        _trackNPCs.Clear();
    }


    // Payment Collect
    private List<NPC_Controller> PayAvailable_NPCs()
    {
        List<NPC_Controller> allNPCs = Main_Controller.instance.All_NPCs();
        List<NPC_Controller> availableNPCs = new();

        for (int i = 0; i < allNPCs.Count; i++)
        {
            if (allNPCs[i].TryGetComponent(out NPC_FoodInteraction interaction) == false) continue;
            if (interaction.payAvailable == false) continue;

            availableNPCs.Add(allNPCs[i]);
        }

        return availableNPCs;
    }
    
    private void Collect(bool activate)
    {
        if (activate == false) return;
        Collect();
    }

    private void Collect()
    {
        if (_coroutine != null) return;
        if (_controller.Food_Icon().Is_MaxAmount()) return;

        _coroutine = StartCoroutine(Collect_Coroutine());
    }
    private IEnumerator Collect_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_searchTime);

            for (int i = 0; i < PayAvailable_NPCs().Count; i++)
            {
                NPC_Movement movement = PayAvailable_NPCs()[i].movement;
                NPC_FoodInteraction interaction = PayAvailable_NPCs()[i].foodInteraction;

                if (_controller.Food_Icon().Is_MaxAmount(interaction.Set_Payment()))
                {
                    if (movement.roamActive) continue;
                    interaction.Update_RoamArea();
                    
                    continue;
                }

                Track_NPC(PayAvailable_NPCs()[i]);
                
                movement.Stop_FreeRoam();
                movement.Assign_TargetPosition(transform.position);
                
                if (movement.At_TargetPosition(transform.position) == false) continue;

                Insert(interaction.Set_Payment());
                interaction.Collect_Payment(0);

                Toggle_AmountBar();
            }

            Refresh_TrackNPCs();
        }
    }


    // Donate
    private List<NPC_Controller> DonateAvailable_NPCs()
    {
        List<NPC_Controller> allNPCs = Main_Controller.instance.All_NPCs();
        List<NPC_Controller> availableNPCs = new();

        for (int i = 0; i < allNPCs.Count; i++)
        {
            if (allNPCs[i].TryGetComponent(out NPC_FoodInteraction interaction) == false) continue;
            
            if (interaction.timeCoroutine != null) continue;
            if (interaction.transferCoroutine != null) continue;
            if (interaction.payAvailable) continue;
            
            if (allNPCs[i].movement.isLeaving) continue;
            
            availableNPCs.Add(allNPCs[i]);
        }
        return availableNPCs;
    }
    
    private void Donate_Collect(bool activate)
    {
        if (activate == false) return;
        Donate_Collect();
    }

    private void Donate_Collect()
    {
        if (_donateCoroutine != null) return;
        if (_controller.Food_Icon().Is_MaxAmount()) return;
        
        _donateCoroutine = StartCoroutine(Donate_Coroutine());
    }
    private IEnumerator Donate_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_donateTime);

            int donateAmount = Random.Range(0, _donateGoldAmount);
            if (_controller.Food_Icon().Is_MaxAmount(donateAmount)) continue;

            List<NPC_Controller> availableNPCs = DonateAvailable_NPCs();
            if (availableNPCs.Count == 0) continue;
            
            NPC_Controller targetNPC = availableNPCs[Random.Range(0, availableNPCs.Count)];
            NPC_Movement movement = targetNPC.movement;

            Track_NPC(targetNPC);
            
            movement.Stop_FreeRoam();
            movement.Assign_TargetPosition(transform.position);

            while (movement.At_TargetPosition() == false) yield return null;
            
            Refresh_TrackNPCs();
            
            if (movement.At_TargetPosition(transform.position) == false) continue;
            if (_controller.Food_Icon().Is_MaxAmount(donateAmount)) continue;
            
            GoldSystem goldSystem = GoldSystem.instance;
            Sprite nuggetSprite = goldSystem.defaultIcon;

            CoinLauncher launcher = targetNPC.itemLauncher;
            
            launcher.Parabola_CoinLaunch(nuggetSprite, transform.position);
            Insert(donateAmount);
            
            movement.CurrentLocation_FreeRoam(launcher.range);
        }
    }
}