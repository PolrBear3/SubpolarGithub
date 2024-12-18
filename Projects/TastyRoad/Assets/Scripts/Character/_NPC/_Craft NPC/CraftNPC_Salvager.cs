using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftNPC_Salvager : CraftNPC
{
    // Current Instance
    public void Set_Instance()
    {
        if (controller.currentCraftNPC != this) return;

        AmountBar nuggetBar = controller.nuggetBar;

        nuggetBar.Set_Amount(ES3.Load("CraftNPC_Salvager/nuggetAmount", controller.nuggetBar.currentAmount));
        nuggetBar.Load();

        FoodData_Controller foodIcon = controller.controller.foodIcon;
        AmountBar timeBar = controller.timeBar;

        foodIcon.Update_AllDatas(ES3.Load("CraftNPC_Salvager/foodData", foodIcon.AllDatas()));
        timeBar.Set_Amount(controller.controller.foodIcon.AllDatas().Count);
        timeBar.Load();

        // subscriptions
        ActionBubble_Interactable interactable = controller.controller.interactable;
        interactable.OnHoldIInteract += Interact_Check;
    }

    public void Save_Instacne()
    {
        if (controller.currentCraftNPC != this) return;

        ES3.Save("CraftNPC_Salvager/nuggetAmount", controller.nuggetBar.currentAmount);
        ES3.Save("CraftNPC_Salvager/foodData", controller.controller.foodIcon.AllDatas());

        // subscriptions
        ActionBubble_Interactable interactable = controller.controller.interactable;
        interactable.OnHoldIInteract -= Interact_Check;
    }


    //
    private void Interact_Check()
    {
        Debug.Log("CraftNPC_Salvager");
    }
}
