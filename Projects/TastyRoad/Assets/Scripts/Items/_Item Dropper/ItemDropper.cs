using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    private Main_Controller _main;

    [Header("")]
    [SerializeField] private Sprite _launchSprite;

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

        _coroutine = StartCoroutine(Drop_Item_Coroutine());
    }
    private IEnumerator Drop_Item_Coroutine()
    {
        Transform playerTransform = _main.Player().transform;

        // spawn
        GameObject itemGameObject = Instantiate(_dropItem, playerTransform.position, Quaternion.identity);
        itemGameObject.transform.SetParent(_main.otherFile);

        // set to nearest snap position


        // launch
        GameObject launchCoin = _launcher.Parabola_CoinLaunch(_launchSprite, playerTransform.position).gameObject;

        // hide
        DropItem dropItem = itemGameObject.GetComponent<DropItem>();
        dropItem.sr.color = Color.clear;

        // show if launch is complete
        while (launchCoin != null) yield return null;
        dropItem.sr.color = Color.white;

        _coroutine = null;
        yield break;
    }
}
