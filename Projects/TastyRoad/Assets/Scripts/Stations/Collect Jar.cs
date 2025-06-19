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


    private Coroutine _coroutine;


    // MonoBehaviour
    private void Start()
    {
        Update_Sprite();
        Toggle_AmountBar();

        Collect(_controller.movement.enabled == false);

        // subscriptions
        Main_Controller.instance.OnFoodBookmark += Collect;
        _controller.movement.OnLoadPosition += Collect;

        Detection_Controller detection = _controller.detection;

        detection.EnterEvent += Toggle_AmountBar;
        detection.ExitEvent += Toggle_AmountBar;

        IInteractable_Controller iInteractable = _controller.iInteractable;

        iInteractable.OnInteract += Toggle_GoldAmount;
        iInteractable.OnHoldInteract += Empty;
    }

    private void OnDestroy()
    {
        Empty();

        // subscriptions
        Main_Controller.instance.OnFoodBookmark -= Collect;
        _controller.movement.OnLoadPosition -= Collect;

        Detection_Controller detection = _controller.detection;

        detection.EnterEvent -= Toggle_AmountBar;
        detection.ExitEvent -= Toggle_AmountBar;

        IInteractable_Controller iInteractable = _controller.iInteractable;

        iInteractable.OnInteract -= Toggle_GoldAmount;
        iInteractable.OnHoldInteract -= Empty;
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


    // NPC
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

                movement.Stop_FreeRoam();
                movement.Assign_TargetPosition(transform.position);

                if (movement.At_TargetPosition(transform.position) == false) continue;

                Insert(interaction.Set_Payment());
                interaction.Collect_Payment(0);

                Toggle_AmountBar();
            }
        }
    }
}