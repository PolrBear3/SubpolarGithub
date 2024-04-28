using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrapGrinder : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Animator _anim;

    private Shop_Controller _controller;
    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private Station_ScrObj _scrap;

    [Header("Current Scrap Amount")]
    [SerializeField] private GameObject _currentScrapIndicator;
    [SerializeField] private TextMeshPro _amountText;

    [Header("")]
    [SerializeField] private float _grindDelayTime;
    


    // UnityEngine
    private void Awake()
    {
        _controller = gameObject.GetComponent<Shop_Controller>();

        _controller.Action1_Event += Grind_Scrap;
        _controller.Interact_Event += CurrentScrapIndicator_Toggle;
        _controller.UnInteract_Event += CurrentScrapIndicator_Toggle;
    }



    /// <summary>
    /// Shows current scrap amount in vehcile when action bubble is active
    /// </summary>
    private void CurrentScrapIndicator_Toggle()
    {
        _currentScrapIndicator.SetActive(_controller.bubble.bubbleOn);

        if (_controller.bubble.bubbleOn == false) return;

        CurrentScrapText_Update();
    }

    private void CurrentScrapText_Update()
    {
        int scrapAmount = _controller.mainController.currentVehicle.menu.stationMenu.Station_Amount(_scrap);

        _amountText.text = scrapAmount.ToString();
    }



    //
    private ItemSlot Scrap_ItemSlot()
    {
        StationMenu_Controller menu = _controller.mainController.currentVehicle.menu.stationMenu;
        List<ItemSlot> stationMenuSlots = menu.slotsController.itemSlots;

        for (int i = 0; i < stationMenuSlots.Count; i++)
        {
            if (stationMenuSlots[i].data.hasItem == false) continue;
            if (stationMenuSlots[i].data.currentStation != _scrap) continue;
            return stationMenuSlots[i];
        }

        return null;
    }

    private void StationCoin_Exchange()
    {
        // grind scrap from vehicle station menu
        Scrap_ItemSlot().Empty_ItemBox();

        // give station coin
        Main_Controller.currentStationCoin += 3;
    }



    private void Grind_Scrap()
    {
        if (Scrap_ItemSlot() == null) return;

        StartCoroutine(Grind_Scrap_Coroutine());
    }
    private IEnumerator Grind_Scrap_Coroutine()
    {
        Player_Controller player = _controller.detection.player;

        // play grind animation
        _anim.Play("ScrapGrinder_grind");

        // disable player control
        player.Player_Input().enabled = false;

        yield return new WaitForSeconds(_grindDelayTime);

        // give station coin effect
        Coin_ScrObj stationCoin = _controller.mainController.dataController.coinTypes[1];
        Vector2 playerDirection = transform.position - _controller.detection.player.transform.position;

        _launcher.Parabola_CoinLaunch(stationCoin, -playerDirection);

        // wait until player recieves station coin
        yield return new WaitForSeconds(_launcher.range);

        // stop grind animation
        _anim.Play("ScrapGrinder_stop");

        // inable player control
        player.Player_Input().enabled = true;

        // scrap grind complete
        StationCoin_Exchange();
    }
}
