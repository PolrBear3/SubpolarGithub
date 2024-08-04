using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasPump : MonoBehaviour
{
    [SerializeField] private ActionBubble_Interactable _interactable;
    [SerializeField] private AmountBar _amountBar;

    [SerializeField] private Station_ScrObj _oilDrum;

    [Header("")]
    [SerializeField] private int _price;

    private Coroutine _coroutine;


    // UnityEngine
    private void Awake()
    {
        
    }

    private void Start()
    {
        _interactable.bubble.Set_Bubble(_oilDrum.miniSprite, null);
        _amountBar.Transparent_Toggle(true);
    }

    private void OnDestroy()
    {
        
    }


    // Functions
    private void Purchase()
    {
        if (_coroutine != null) return;

        Main_Controller main = _interactable.mainController;
        if (main.GoldenNugget_Amount() < _price) return;

        Charge_AmountBar();
    }

    private void Charge_AmountBar()
    {
        _coroutine = StartCoroutine(Charge_AmountBar_Coroutine());
    }
    private IEnumerator Charge_AmountBar_Coroutine()
    {
        int maxBarCount = _amountBar.amountBarSprite.Length - 1;

        for (int i = 0; i < maxBarCount; i++)
        {
            _amountBar.Update_Amount(1);
            _amountBar.Load();

            yield return new WaitForSeconds(1);
        }

        _coroutine = null;
        yield break;
    }
}
