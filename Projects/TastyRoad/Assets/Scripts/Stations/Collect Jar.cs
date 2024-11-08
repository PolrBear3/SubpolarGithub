using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectJar : Stack_Table, IInteractable
{
    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private Sprite[] _jarSprites;

    [Header("")]
    [SerializeField] private Food_ScrObj _collectFood;
    [SerializeField][Range(1, 10)] private int _retrieveAmount;

    private Coroutine _coroutine;


    // UnityEngine
    private new void Start()
    {
        GlobalTime_Controller.TimeTik_Update += CollectNuggets;

        FoodData_Controller foodIcon = stationController.Food_Icon();

        // deactivate rotten system
        foodIcon.gameObject.GetComponent<FoodData_RottenSystem>().enabled = false;

        Update_JarSprite();

        if (foodIcon.hasFood == true) return;

        // set basic data
        foodIcon.Set_CurrentData(new FoodData(_collectFood));
    }

    private new void OnDestroy()
    {
        Retrieve_All();

        GlobalTime_Controller.TimeTik_Update -= CollectNuggets;
    }


    // IInteractable
    public new void Interact()
    {
        Retrieve();
    }


    // Gets and Checks
    /// <returns>
    /// All NPCs with _collectFood
    /// </returns>
    private List<NPC_Controller> CollectFood_NPCs()
    {
        List<NPC_Controller> allNPCs = stationController.mainController.All_NPCs();
        List<NPC_Controller> targetNPCs = new();

        for (int i = 0; i < allNPCs.Count; i++)
        {
            FoodData_Controller foodIcon = allNPCs[i].foodIcon;

            if (foodIcon.hasFood == false) continue;
            if (foodIcon.headData.foodScrObj != _collectFood) continue;

            targetNPCs.Add(allNPCs[i]);
        }

        return targetNPCs;
    }

    private List<NPC_Controller> FoodOrderServed_NPCs()
    {
        List<NPC_Controller> allNPCs = stationController.mainController.All_NPCs();
        List<NPC_Controller> servedNPCs = new();

        for (int i = 0; i < allNPCs.Count; i++)
        {
            if (allNPCs[i].foodIcon.hasFood == false) continue;
            if (allNPCs[i].interaction.FoodOrder_Served() == false) continue;

            servedNPCs.Add(allNPCs[i]);
        }

        return servedNPCs;
    }

    private NPC_Controller Closest_NPC(List<NPC_Controller> compareNPCs)
    {
        NPC_Controller closestNPC = null;
        float closestDistance = Mathf.Infinity;

        foreach (NPC_Controller npc in compareNPCs)
        {
            float distance = Vector2.Distance(transform.position, npc.transform.position);

            if (distance > closestDistance) continue;

            closestDistance = distance;
            closestNPC = npc;
        }

        return closestNPC;
    }


    private bool Jar_Empty()
    {
        FoodData_Controller foodIcon = stationController.Food_Icon();
        return foodIcon.hasFood == false || foodIcon.headData.currentAmount <= 1;
    }


    // Sprite Control
    private void Update_JarSprite()
    {
        SpriteRenderer sr = stationController.spriteRenderer;

        if (Jar_Empty() == true)
        {
            sr.sprite = _jarSprites[0];
            return;
        }

        sr.sprite = _jarSprites[1];
    }


    // Functions
    private void Retrieve()
    {
        if (Jar_Empty() == true) return;

        FoodMenu_Controller foodMenu = stationController.mainController.currentVehicle.menu.foodMenu;

        stationController.Food_Icon().headData.Update_Amount(-_retrieveAmount);
        foodMenu.Add_FoodItem(_collectFood, _retrieveAmount);

        Update_JarSprite();

        Transform launchDirection = stationController.detection.player.transform;
        _launcher.Parabola_CoinLaunch(_collectFood.sprite, launchDirection.position);
    }

    private void Retrieve_All()
    {
        if (Jar_Empty() == true) return;

        FoodMenu_Controller foodMenu = stationController.mainController.currentVehicle.menu.foodMenu;
        foodMenu.Add_FoodItem(_collectFood, stationController.Food_Icon().headData.currentAmount - 1);
    }


    private void CollectNuggets()
    {
        if (_coroutine != null) return;
        if (FoodOrderServed_NPCs().Count <= 0) return;

        _coroutine = StartCoroutine(CollectNuggets_Coroutine());
    }
    private IEnumerator CollectNuggets_Coroutine()
    {
        NPC_Controller targetNPC = Closest_NPC(FoodOrderServed_NPCs());
        NPC_Movement movement = targetNPC.movement;
        NPC_Interaction interaction = targetNPC.interaction;

        // move to current jar
        movement.Stop_FreeRoam();
        movement.Assign_TargetPosition(transform.position);

        // wait until arrival
        while (movement.At_TargetPosition(transform.position) == false)
        {
            // cancel function if player collects nugget
            if (interaction.FoodOrder_Served() == false)
            {
                _coroutine = null;
                CollectNuggets();

                yield break;
            }

            yield return null;
        }

        // add to current food data
        stationController.Food_Icon().headData.Update_Amount(interaction.foodScore);

        // clear data and leave
        interaction.Clear_Data();

        SpriteRenderer currentLocation = stationController.mainController.currentLocation.roamArea;

        movement.Update_RoamArea(currentLocation);
        movement.Leave(movement.Random_IntervalTime());

        // launch nugget from npc to current jar
        targetNPC.itemLauncher.Parabola_CoinLaunch(_collectFood.sprite, transform.position);

        //
        Update_JarSprite();

        _coroutine = null;
        CollectNuggets();

        yield break;
    }
}