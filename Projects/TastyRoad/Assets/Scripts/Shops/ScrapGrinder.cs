using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapGrinder : MonoBehaviour
{
    [SerializeField] private Animator _anim;

    private Shop_Controller _controller;
    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private float _grindDelayTime;
    
    
    
    // UnityEngine
    private void Awake()
    {
        _controller = gameObject.GetComponent<Shop_Controller>();

        _controller.Action1_Event += Grind_Scrap;
    }



    //
    private ItemSlot Scrap_ItemSlot()
    {
        StationMenu_Controller menu = _controller.mainController.currentVehicle.menu.stationMenu;

        for (int i = 0; i < menu.ItemSlots().Count; i++)
        {
            if (menu.ItemSlots()[i].data.hasItem == false) continue;
            if (menu.ItemSlots()[i].data.currentStation.id != 6) continue;
            return menu.ItemSlots()[i];
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
