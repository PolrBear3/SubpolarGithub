using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Grocery : MonoBehaviour, IInteractable, ISaveLoadable
{
    private PlayerInput _input;

    private Main_Controller _main;
    private Detection_Controller _detection;

    [SerializeField] private Action_Bubble _bubble;
    [SerializeField] private CoinLauncher _launcher;

    private int _hoverNum;
    private List<FoodData> _purchasableFoods = new();

    [Header("")]
    [SerializeField] private Vector2 _claimPositionRange;

    [Header("Hovering Food")]
    [SerializeField] private GameObject _hoverControlKeys;
    [SerializeField] private SpriteRenderer _hoveringFoodSR;
    [SerializeField] private Animator _hoveringFoodAnim;

    [Header("Price Indicator")]
    [SerializeField] private GameObject _priceIndicator;
    [SerializeField] private TextMeshPro _priceText;



    // UnityEngine
    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();

        _main = FindObjectOfType<Main_Controller>();
        _detection = gameObject.GetComponent<Detection_Controller>();

        Claim_Position();
    }

    private void Start()
    {
        if (ES3.KeyExists("Grocery_purchasableFoods") == false)
        {
            Sort_AvailableFoods();
        }

        Update_HoverFood();
        UnInteract();
    }



    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        _input.enabled = true;

        _hoveringFoodAnim.Play("TransparencyBlinker_blink");
        _hoverControlKeys.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller player)) return;

        UnInteract();
    }



    // InputSystem
    private void OnAction1()
    {
        if (_bubble.bubbleOn)
        {
            Purchase_HoverFood();
            return;
        }

        _hoverNum--;
        Update_HoverFood();
    }

    private void OnAction2()
    {
        if (_bubble.bubbleOn) return;

        _hoverNum++;
        Update_HoverFood();
    }



    // IInteractable
    public void Interact()
    {
        if (_bubble.bubbleOn == false)
        {
            _bubble.Toggle(true);
            _hoverControlKeys.SetActive(false);

            Update_HoverFood();
            _priceIndicator.SetActive(true);

            return;
        }

        UnInteract();
    }

    public void UnInteract()
    {
        _bubble.Toggle(false);
        _hoverControlKeys.SetActive(true);

        _priceIndicator.SetActive(false);

        if (_detection.player != null) return;

        _input.enabled = false;

        _hoveringFoodAnim.Play("TransparencyBlinker_hide");
        _hoverControlKeys.SetActive(false);
    }



    // ISaveLoadable
    public void Save_Data()
    {
        if (_purchasableFoods.Count <= 0) return;

        ES3.Save("Grocery_purchasableFoods", _purchasableFoods);
    }

    public void Load_Data()
    {
        _purchasableFoods = ES3.Load("Grocery_purchasableFoods", _purchasableFoods);
    }



    // Position Claim
    private void Claim_Position()
    {
        int claimRepeatNum = (int)_claimPositionRange.x * 2 + 1;
        float positionXNum = transform.position.x - _claimPositionRange.x;

        for (int i = 0; i < claimRepeatNum; i++)
        {
            _main.Claim_Position(new(positionXNum, transform.position.y));
            positionXNum++;
        }
    }



    //
    private void Sort_AvailableFoods()
    {
        List<Food_ScrObj> dataFoods = _main.dataController.rawFoods;

        // remove all cook necessary raw foods
        for (int i = 0; i < dataFoods.Count; i++)
        {
            if (dataFoods[i].ingredients.Count > 0) continue;
            _purchasableFoods.Add(new(dataFoods[i], 99));
        }

        // sort by price from lowest to highest
        _purchasableFoods.Sort((x, y) => x.foodScrObj.price.CompareTo(y.foodScrObj.price));
    }

    private void Update_HoverFood()
    {
        if (_hoverNum < 0) _hoverNum = _purchasableFoods.Count - 1;
        else if (_hoverNum > _purchasableFoods.Count - 1) _hoverNum = 0;

        FoodData currentFood = _purchasableFoods[_hoverNum];

        // update price text
        _priceText.text = currentFood.foodScrObj.price.ToString();

        // update center position
        _hoveringFoodSR.transform.localPosition = currentFood.foodScrObj.centerPosition * 0.01f;

        // update sprite
        _hoveringFoodSR.sprite = currentFood.foodScrObj.sprite;
    }

    private void Purchase_HoverFood()
    {
        FoodData currentFood = _purchasableFoods[_hoverNum];

        // check gold coin
        if (Main_Controller.currentGoldCoin < currentFood.foodScrObj.price)
        {
            // not enough gold coins !!
            return;
        }

        // calculate gold coin
        Main_Controller.currentGoldCoin -= currentFood.foodScrObj.price;

        // add current hovering food to food menu
        _main.currentVehicle.menu.foodMenu.Add_FoodItem(currentFood.foodScrObj, 1);

        // player toss gold coin to grocery npc
        Player_Controller player = _detection.player;
        Coin_ScrObj goldCoin = _main.dataController.coinTypes[0];
        player.coinLauncher.Parabola_CoinLaunch(goldCoin, transform.position - player.transform.position);

        // toss food to player
        _launcher.Parabola_CoinLaunch(currentFood.foodScrObj.sprite, player.transform.position - transform.position);
    }
}
