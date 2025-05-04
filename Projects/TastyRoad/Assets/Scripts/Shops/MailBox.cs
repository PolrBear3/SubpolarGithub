using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailBox : MonoBehaviour
{
    private SpriteRenderer _sr;


    [Header("")]
    [SerializeField] private IInteractable_Controller _interactable;
    [SerializeField] private Detection_Controller _detection;
    [SerializeField] private DialogTrigger _dialog;

    [Header("")]
    [SerializeField] private AmountBar _amountBar;

    [Header("")]
    [SerializeField] private Sprite[] _sprites;

    [Header("")]
    [SerializeField] private GameObject _collectCard;

    [Header("")]
    [SerializeField][Range(0, 1000)] private int _insertPrice;


    private Coroutine _coroutine;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();

        Load_Data();
    }

    private void Start()
    {
        Update_Sprite();

        _amountBar.Load();
        Toggle_AmountBar();

        // subscriptions
        globaltime.instance.OnTimeTik += Drop_CollectCard;

        _detection.EnterEvent += Toggle_AmountBar;
        _detection.ExitEvent += Toggle_AmountBar;

        _interactable.OnInteract += Toggle_Price;
        _interactable.OnHoldInteract += Purchase_Insert;
    }

    private void OnDestroy()
    {
        Save_Data();

        // subscriptions
        globaltime.instance.OnTimeTik -= Drop_CollectCard;

        _detection.EnterEvent -= Toggle_AmountBar;
        _detection.ExitEvent -= Toggle_AmountBar;

        _interactable.OnInteract -= Toggle_Price;
        _interactable.OnHoldInteract -= Purchase_Insert;
    }


    // ISaveLoadable
    private void Save_Data()
    {
        ES3.Save("MailBox/AmountBar/_currentAmount", _amountBar.currentAmount);
    }

    private void Load_Data()
    {
        _amountBar.Set_Amount(ES3.Load("MailBox/AmountBar/_currentAmount", _amountBar.currentAmount));
    }


    // Visual Control
    private void Update_Sprite()
    {
        if (_amountBar.currentAmount <= 0)
        {
            _sr.sprite = _sprites[0];
            return;
        }

        _sr.sprite = _sprites[1];
    }


    public void Toggle_Price()
    {
        GoldSystem.instance.Indicate_TriggerData(new(_dialog.defaultData.icon, -_insertPrice));
    }

    public void Toggle_AmountBar()
    {
        _amountBar.Toggle(_detection.player != null);
    }


    // NPC Control
    private NPC_Controller Spawn_MailNPC()
    {
        Main_Controller main = Main_Controller.instance;

        // get current location outer position
        Location_Controller currentLocation = main.currentLocation;
        Vector2 spawnPoint = currentLocation.OuterLocation_Position(Random.Range(0, 2));

        // spawn
        GameObject spawnNPC = main.Spawn_Character(2, spawnPoint);
        NPC_Controller npc = spawnNPC.GetComponent<NPC_Controller>();

        main.UnTrack_CurrentCharacter(spawnNPC);

        return npc;
    }


    // Spawn Points
    private Vector2 Available_SpawnPoint()
    {
        List<Vector2> spawnPoints = new();

        spawnPoints.Add(new Vector2(transform.position.x + 1, transform.position.y));
        spawnPoints.Add(new Vector2(transform.position.x - 1, transform.position.y));

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (Main_Controller.instance.Position_Claimed(spawnPoints[i]) == true) continue;
            return spawnPoints[i];
        }

        return transform.position;
    }


    // Interact Functions
    private void Purchase_Insert()
    {
        if (_amountBar.Is_MaxAmount()) return;
        if (GoldSystem.instance.Update_CurrentAmount(-_insertPrice) == false) return;

        // increase amount
        _amountBar.Update_Amount(1);
        _amountBar.Load();

        Update_Sprite();

        // sfx
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
    }

    private void Drop_CollectCard()
    {
        if (_coroutine != null) return;
        if (_amountBar.currentAmount <= 0) return;

        _coroutine = StartCoroutine(Drop_CollectCard_Coroutine());
    }
    private IEnumerator Drop_CollectCard_Coroutine()
    {
        NPC_Movement movement = Spawn_MailNPC().movement;

        // move to current mail box
        movement.Assign_TargetPosition(transform.position);
        while (movement.At_TargetPosition() == false) yield return null;

        Main_Controller main = Main_Controller.instance;

        // check if spawn point is empty
        if (Available_SpawnPoint() != (Vector2)transform.position)
        {
            // spawn collect card
            GameObject collectCard = Instantiate(_collectCard, Available_SpawnPoint(), Quaternion.identity);
            collectCard.transform.SetParent(main.otherFile);

            // decrease amount
            _amountBar.Update_Amount(-1);
            _amountBar.Load();

            Update_Sprite();
        }

        // get current location outer position
        Location_Controller currentLocation = main.currentLocation;

        // leave
        int direction = Random.Range(0, 2);
        Vector2 spawnPoint = currentLocation.OuterLocation_Position(direction);

        movement.Assign_TargetPosition(spawnPoint);

        while (movement.At_TargetPosition() == false) yield return null;
        movement.Leave(direction, 0f);

        _coroutine = null;
        yield break;
    }
}