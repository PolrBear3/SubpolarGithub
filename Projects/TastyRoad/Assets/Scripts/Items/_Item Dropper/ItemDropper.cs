using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    private Main_Controller _main;

    [Header("")]
    [SerializeField] private CoinLauncher _launcher;

    [Header("")]
    [SerializeField] private GameObject _dropItem;
    [SerializeField] private GameObject _collectCard;

    [Header("")]
    [SerializeField] private Sprite _defaultLaunchSprite;

    private Coroutine _coroutine;


    // UnityEngine
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }


    // Launch
    private IEnumerator Launch_ShowItem(SpriteRenderer dropItemSR, Sprite launchSprite)
    {
        // hide
        dropItemSR.color = Color.clear;

        // launch
        Transform playerTransform = _main.Player().transform;
        GameObject launchCoin = _launcher.Parabola_CoinLaunch(launchSprite, playerTransform.position).gameObject;

        // show
        while (launchCoin != null) yield return null;
        dropItemSR.color = Color.white;

        _coroutine = null;
        yield break;
    }


    // Drops
    public void Drop_Food(Food_ScrObj dropFood, int amount)
    {
        if (_coroutine != null) return;

        Transform playerTransform = _main.Player().transform;
        Vector2 dropPosition = Main_Controller.SnapPosition(playerTransform.position);

        if (_main.Position_Claimed(dropPosition)) return;

        // spawn to nearest snap position
        GameObject itemGameObject = Instantiate(_dropItem, dropPosition, Quaternion.identity);
        itemGameObject.transform.SetParent(_main.otherFile);

        // set drop data
        DropItem dropItem = itemGameObject.GetComponent<DropItem>();
        dropItem.Set_ItemData(new ItemSlot_Data(dropFood, amount));

        StartCoroutine(Launch_ShowItem(dropItem.sr, _defaultLaunchSprite));
    }

    public void Drop_CollectCard()
    {
        if (_coroutine != null) return;

        Transform playerTransform = _main.Player().transform;
        Vector2 dropPosition = Main_Controller.SnapPosition(playerTransform.position);

        if (_main.Position_Claimed(dropPosition)) return;

        // spawn to nearest snap position
        GameObject itemGameObject = Instantiate(_collectCard, dropPosition, Quaternion.identity);
        CollectCard dropCard = itemGameObject.GetComponent<CollectCard>();
        itemGameObject.transform.SetParent(_main.otherFile);

        StartCoroutine(Launch_ShowItem(dropCard.sr, dropCard.launchSprite));
    }
}
