using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_GiftSystem : MonoBehaviour
{
    [Header("")]
    [SerializeField] private NPC_Controller _controller;

    [Header("")]
    [SerializeField] private GameObject _giftCoolTimeBar;
    [SerializeField] private AmountBar _coolTimeBar;

    [Header("")]
    [SerializeField][Range(0, 100)] private float _itemDropRate;
    [SerializeField][Range(0, 100)] private float _collectCardDropRate;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _dropAmountRange;

    private Coroutine _coroutine;


    // UnityEngine
    private void Start()
    {
        _controller.interactable.OnHoldInteract += ToggleBar_Duration;
    }

    private void OnDestroy()
    {
        _controller.interactable.OnHoldInteract -= ToggleBar_Duration;
    }


    //
    private void ToggleBar_Duration()
    {
        if (_coroutine != null) return;

        _coroutine = StartCoroutine(ToggleBar_Duration_Coroutine());
    }
    private IEnumerator ToggleBar_Duration_Coroutine()
    {


        _coroutine = null;
        yield break;
    }
}
