using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sr;

    private Station_Controller _stationController;
    public Station_Controller stationController => _stationController;

    [SerializeField] private Action_Bubble _actionBubble;

    [Header("")]
    [SerializeField] private List<Sprite> _scrapSprites = new();



    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _stationController = gameObject.GetComponent<Station_Controller>();
    }

    private void Start()
    {
        Set_RandomSprite();
    }



    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_stationController.movement.enabled) return;
        if (!collision.TryGetComponent(out Player_Controller player)) return;
        UnInteract();
    }



    // IInteractable
    public void Interact()
    {
        if (_actionBubble.bubbleOn)
        {
            UnInteract();
            return;
        }

        _actionBubble.Toggle(true);

        _stationController.PlayerInput_Activation(true);
        _stationController.Action1_Event += Collect_toVehicle;
    }

    public void UnInteract()
    {
        _actionBubble.Toggle(false);

        _stationController.PlayerInput_Activation(false);
        _stationController.Action1_Event -= Collect_toVehicle;
    }



    //
    private void Set_RandomSprite()
    {
        int randArrayNum = Random.Range(0, _scrapSprites.Count);
        _sr.sprite = _scrapSprites[randArrayNum];
    }

    private void Collect_toVehicle()
    {
        StationMenu_Controller menu = _stationController.mainController.currentVehicle.menu.stationMenu;

        // check if station slots are all full
        if (menu.Slots_Full())
        {
            // slots all full animation ?

            UnInteract();
            return;
        }

        // add this station to vehicle station menu
        menu.Add_StationItem(_stationController.stationScrObj, 1);

        // destroy
        _stationController.Destroy_Station();
    }
}
