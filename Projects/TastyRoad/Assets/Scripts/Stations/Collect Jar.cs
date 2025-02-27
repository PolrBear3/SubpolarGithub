using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectJar : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Station_Controller _controller;

    [Header("")]
    [SerializeField] private Sprite[] _jarSprites;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _searchTime;


    private Coroutine _coroutine;


    // MonoBehaviour
    private void Start()
    {
        Update_Sprite();

        Collect(_controller.movement.enabled == false);

        // subscriptions
        Main_Controller.instance.OnFoodBookmark += Collect;
        _controller.movement.OnLoadPosition += Collect;

        Detection_Controller detection = _controller.detection;

        detection.EnterEvent += Toggle_Indications;
        detection.ExitEvent += Toggle_Indications;

        IInteractable_Controller iInteractable = _controller.iInteractable;

        iInteractable.OnInteract += Insert;
        iInteractable.OnInteract += Update_Sprite;

        iInteractable.OnHoldInteract += Transfer;
    }

    private void OnDestroy()
    {
        Retrieve();

        // subscriptions
        Main_Controller.instance.OnFoodBookmark -= Collect;
        _controller.movement.OnLoadPosition -= Collect;

        Detection_Controller detection = _controller.detection;

        detection.EnterEvent -= Toggle_Indications;
        detection.ExitEvent -= Toggle_Indications;

        IInteractable_Controller iInteractable = _controller.iInteractable;

        iInteractable.OnInteract -= Insert;
        iInteractable.OnInteract -= Update_Sprite;

        iInteractable.OnHoldInteract -= Transfer;
    }


    // Indications
    private void Update_Sprite()
    {
        if (_controller.Food_Icon().hasFood == false)
        {
            _controller.spriteRenderer.sprite = _jarSprites[0];
            return;
        }

        _controller.spriteRenderer.sprite = _jarSprites[1];
    }

    private void Toggle_Indications()
    {
        _controller.Food_Icon().Toggle_AmountBar(_controller.detection.player != null);
    }


    // Main
    private void Retrieve()
    {
        FoodData_Controller foodIcon = _controller.Food_Icon();

        if (foodIcon.hasFood == false) return;

        FoodMenu_Controller foodMenu = _controller.mainController.currentVehicle.menu.foodMenu;
        Food_ScrObj nugget = foodIcon.currentData.foodScrObj;

        foodMenu.Add_FoodItem(nugget, foodIcon.currentData.currentAmount);
    }


    // NPC
    private List<NPC_Controller> PayAvailable_NPCs()
    {
        List<NPC_Controller> allNPCs = _controller.mainController.All_NPCs();
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
                interaction.Collect_Payment();
            }
        }
    }


    private void Insert(int insertAmount)
    {
        if (insertAmount <= 0) return;

        FoodData_Controller stationIcon = _controller.Food_Icon();
        if (stationIcon.Is_MaxAmount()) return;

        Food_ScrObj nugget = _controller.mainController.dataController.goldenNugget;
        stationIcon.Update_Amount(nugget, insertAmount);

        Toggle_Indications();
        Update_Sprite();

        // sfx
        Audio_Controller.instance.Play_OneShot(gameObject, 2);
    }


    // Player
    private void Insert()
    {
        Food_ScrObj nugget = _controller.mainController.dataController.goldenNugget;
        FoodData_Controller playerIcon = _controller.detection.player.foodIcon;

        if (playerIcon.Is_SameFood(nugget) == false)
        {
            Transfer();
            return;
        }

        FoodData_Controller stationIcon = _controller.Food_Icon();
        if (stationIcon.Is_MaxAmount()) return;

        Insert(1);

        playerIcon.Set_CurrentData(null);
        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();
    }


    private bool Transfer_Available()
    {
        FoodData_Controller stationIcon = _controller.Food_Icon();

        if (stationIcon.hasFood == false) return false;
        if (stationIcon.currentData.currentAmount <= 0) return false;

        return true;
    }

    private void Transfer()
    {
        if (Transfer_Available() == false) return;

        FoodData_Controller stationIcon = _controller.Food_Icon();
        FoodData_Controller playerIcon = _controller.detection.player.foodIcon;

        Food_ScrObj nugget = _controller.mainController.dataController.goldenNugget;

        for (int i = 0; i < _controller.Food_Icon().maxAmount; i++)
        {
            if (Transfer_Available() == false) break;
            if (playerIcon.DataCount_Maxed()) break;

            playerIcon.Set_CurrentData(new(nugget));
            stationIcon.Update_Amount(nugget, -1);
        }

        playerIcon.Show_Icon();
        playerIcon.Toggle_SubDataBar(true);
        playerIcon.Show_Condition();

        Update_Sprite();
        Toggle_Indications();

        Collect();

        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 1);
    }
}