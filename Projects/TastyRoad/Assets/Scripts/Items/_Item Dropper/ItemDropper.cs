using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    private Main_Controller _main;

    [Header("")]
    [SerializeField] private CoinLauncher _launcher;
    [SerializeField] private GameObject _dropItem;

    private Coroutine _coroutine;


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }


    //
    public void Drop_Item()
    {
        if (_coroutine != null) return;

        Transform playerTransform = _main.Player().transform;
        Vector2 dropPosition = Main_Controller.SnapPosition(playerTransform.position);

        if (_main.Position_Claimed(dropPosition)) return;

        _coroutine = StartCoroutine(Drop_Item_Coroutine());
    }
    private IEnumerator Drop_Item_Coroutine()
    {
        Transform playerTransform = _main.Player().transform;
        Vector2 spawnPosition = Main_Controller.SnapPosition(playerTransform.position);

        // spawn to nearest snap position
        GameObject itemGameObject = Instantiate(_dropItem, spawnPosition, Quaternion.identity);
        itemGameObject.transform.SetParent(_main.otherFile);

        // hide
        DropItem dropItem = itemGameObject.GetComponent<DropItem>();
        dropItem.sr.color = Color.clear;

        // launch
        GameObject launchCoin = _launcher.Parabola_CoinLaunch(dropItem.launchSprite, playerTransform.position).gameObject;

        // show
        while (launchCoin != null) yield return null;
        dropItem.sr.color = Color.white;

        _coroutine = null;
        yield break;
    }
}
