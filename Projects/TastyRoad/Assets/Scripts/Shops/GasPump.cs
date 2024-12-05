using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPump : MonoBehaviour
{
    [SerializeField] private ActionBubble_Interactable _interactable;
    [SerializeField] private AmountBar _amountBar;
    [SerializeField] private CoinLauncher _launcher;

    [SerializeField] private Station_ScrObj _oilDrum;

    [Header("")]
    [SerializeField] private Sprite _purchaseSprite;
    [SerializeField] private int _price;

    private bool _collectReady;

    private Coroutine _coroutine;


    // UnityEngine
    private void Start()
    {
        Update_ActionBubble_Icon();

        _amountBar.Set_Amount(0);
        _amountBar.Load();
        _amountBar.Toggle(false);

        // Subscriptions
        _interactable.detection.EnterEvent += AmountBar_Toggle;
        _interactable.detection.ExitEvent += AmountBar_Toggle;

        _interactable.InteractEvent += Update_Dialog;

        _interactable.OnAction1Event += Purchase;
        _interactable.OnAction1Event += Collect;
    }

    private void OnDestroy()
    {
        // Subscriptions
        _interactable.detection.EnterEvent -= AmountBar_Toggle;
        _interactable.detection.ExitEvent -= AmountBar_Toggle;

        _interactable.InteractEvent -= Update_Dialog;

        _interactable.OnAction1Event -= Purchase;
        _interactable.OnAction1Event -= Collect;
    }


    // Indication Updates
    private void Update_ActionBubble_Icon()
    {
        if (_collectReady == false)
        {
            _interactable.bubble.Set_Bubble(_purchaseSprite, null);
            return;
        }

        _interactable.bubble.Set_Bubble(_oilDrum.miniSprite, null);
    }

    private void AmountBar_Toggle()
    {
        if (_coroutine == null && _collectReady == false)
        {
            _amountBar.Toggle(false);
            return;
        }

        _amountBar.Toggle(_interactable.detection.player != null);
    }

    private void Update_Dialog()
    {
        if (_interactable.bubble.bubbleOn == false) return;

        DialogTrigger dialog = gameObject.GetComponent<DialogTrigger>();

        if (_collectReady == true)
        {
            dialog.Update_Dialog(new DialogData(_oilDrum.dialogIcon, dialog.datas[0].info));
            return;
        }

        StationMenu_Controller menu = _interactable.mainController.currentVehicle.menu.stationMenu;

        /*
        if (menu.controller.slotsController.Slots_Full())
        {
            dialog.Update_Dialog(new DialogData(_oilDrum.dialogIcon, dialog.datas[1].info));
            return;
        }
        */

        string priceDialog = _price + " Gold Nuggets for Oil Drum.";
        dialog.Update_Dialog(new DialogData(_purchaseSprite, priceDialog));
    }


    // Functions
    private void Purchase()
    {
        if (_collectReady == true) return;

        Main_Controller main = _interactable.mainController;
        if (main.GoldenNugget_Amount() < _price) return;

        main.Remove_GoldenNugget(_price);
        Charge_AmountBar();

        CoinLauncher playerLauncher = main.Player().coinLauncher;
        Sprite nuggetSprite = main.dataController.goldenNugget.sprite;

        playerLauncher.Parabola_CoinLaunch(nuggetSprite, main.Player().transform.position);
    }

    private void Charge_AmountBar()
    {
        if (_coroutine != null) return;

        _coroutine = StartCoroutine(Charge_AmountBar_Coroutine());
    }
    private IEnumerator Charge_AmountBar_Coroutine()
    {
        _interactable.LockInteract(true);

        _amountBar.Set_Amount(0);
        _amountBar.Toggle(true);

        int maxBarCount = _amountBar.amountBarSprite.Length;

        for (int i = 0; i < maxBarCount; i++)
        {
            yield return new WaitForSeconds(1);

            _amountBar.Update_Amount(1);
            _amountBar.Load();
        }

        _collectReady = true;
        Update_ActionBubble_Icon();

        _interactable.LockInteract(false);

        _amountBar.Toggle_BarColor(true);
        _amountBar.Load();

        _coroutine = null;
        yield break;
    }

    private void Collect()
    {
        if (_collectReady == false) return;

        StationMenu_Controller menu = _interactable.mainController.currentVehicle.menu.stationMenu;

        // slots full check //

        menu.Add_StationItem(_oilDrum, 1);
        _launcher.Parabola_CoinLaunch(_oilDrum.miniSprite, _interactable.detection.player.transform.position);

        _collectReady = false;
        Update_ActionBubble_Icon();

        _amountBar.Toggle(false);
        _amountBar.Toggle_BarColor(false);
    }
}
