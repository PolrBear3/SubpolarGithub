using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Baggage : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private SpriteRenderer _sr;
    private Game_Controller _gameController;

    [SerializeField] private float _speed;

    private NPC_Controller _ownerNPC;
    public NPC_Controller ownerNPC => _ownerNPC;

    private Baggage_CheckPoint _detectedCheckPoint;
    public Baggage_CheckPoint detectedCheckPoint => _detectedCheckPoint;

    public delegate void Event();
    public event Event PointerClick_Event;

    private int _checkNum;
    public int checkNum => _checkNum;

    private bool _dragging;
    public bool dragging => _dragging;

    private bool _moveable;
    public bool moveable => _moveable;

    private int _revealedHeatNum;
    public int revealedHeatNum => _revealedHeatNum;

    [SerializeField] private Baggage_Data _data;
    public Baggage_Data data => _data;

    [SerializeField] private Vector2 _detectChanceRange;
    [SerializeField] private float _detectBonusRate;



    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _gameController = FindObjectOfType<Game_Controller>();
    }

    private void Start()
    {
        Set_Data();
        Set_Sprite();

        PointerClick_Event += HandDetector_Check_HeatLevel;
        PointerClick_Event += HeatLevel3_Arrest;

        ownerNPC.interaction.PointerClick_Event += HeatLevel3_Arrest;
    }

    private void Update()
    {
        if (_moveable == false) return;
        transform.Translate(_speed * Time.deltaTime * new Vector2(1, 0));
    }



    // EventSystems
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Equipment_Controller.currentEquipment != Equipment.hand) return;
        if (_ownerNPC.interaction.hasBaggage) return;

        _gameController.CursorSprite_Update(1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _gameController.CursorSprite_Update(0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Equipment_Controller.currentEquipment != Equipment.hand) return;
        if (_ownerNPC.interaction.hasBaggage) return;

        _dragging = true;

        _gameController.CheckPointsBlink_AnimationToggle(true);

        Movement_Toggle(false);
        transform.parent = _gameController.transform;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(mousePos.x, mousePos.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Equipment_Controller.currentEquipment != Equipment.hand) return;
        if (_ownerNPC.interaction.hasBaggage) return;

        _dragging = false;

        _gameController.CheckPointsBlink_AnimationToggle(false);

        // give back to npc
        if (_ownerNPC.interaction.hasBaggage)
        {
            _ownerNPC.interaction.Set_Baggage(this);
            return;
        }

        // return back to original point
        if (_detectedCheckPoint == null)
        {
            _gameController.checkPoints[_checkNum].Set_Baggage(this);
        }
        // set to new check point
        else
        {
            _gameController.checkPoints[_checkNum].Remove_Baggage(this);

            _detectedCheckPoint.Set_Baggage(this);
            _checkNum = _detectedCheckPoint.checkPointNum;
        }
    }



    // Event Trigger
    public void Pointer_Click()
    {
        PointerClick_Event?.Invoke();
    }

    public void Pointer_Down()
    {
        if (Equipment_Controller.currentEquipment != Equipment.hand) return;
        if (_ownerNPC.interaction.hasBaggage) return;

        FindObjectOfType<Sound_Controller>().Play_Sound("drag");
    }

    public void Pointer_Up()
    {
        if (Equipment_Controller.currentEquipment != Equipment.hand) return;
        if (_ownerNPC.interaction.hasBaggage) return;

        FindObjectOfType<Sound_Controller>().Play_Sound("drop");
    }



    // Ontriggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage_CheckPoint point)) return;

        _detectedCheckPoint = point;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage_CheckPoint point)) return;

        _detectedCheckPoint = null;
    }



    // Toggle Control
    public void Sprite_Toggle(bool toggleOn)
    {
        if (toggleOn)
        {
            _sr.color = Color.white;
        }
        else
        {
            _sr.color = Color.clear;
        }
    }

    public void Movement_Toggle(bool toggleOn)
    {
        _moveable = toggleOn;
    }



    //
    private void Set_Data()
    {
        int randTypeNum = Random.Range(0, _gameController.data.bagSpriteDatas.Count);
        float randChanceRate = Random.Range(_detectChanceRange.x, _detectChanceRange.y);

        _data = new(randTypeNum, randChanceRate);
    }

    private void Set_Sprite()
    {
        _sr.sprite = _gameController.data.BaggageSprite(_data.typeNum, 0);
    }

    public void Set_OwnerNPC(NPC_Controller owner)
    {
        _ownerNPC = owner;
    }



    // for Machine Detector
    public void Check_HeatLevel()
    {
        if (_data.heatLevel <= 0) return;
        // if detect chance percentage
        if (_revealedHeatNum + 1 > _data.heatLevel) return;

        _revealedHeatNum++;
        _sr.sprite = _gameController.data.BaggageSprite(_data.typeNum, _revealedHeatNum);
    }

    // for Hand Detector
    private void HandDetector_Check_HeatLevel()
    {
        // if hand detector attached
        if (Equipment_Controller.currentEquipment != Equipment.detector) return;

        // battery count check
        if (_gameController.equipment.batteryCount <= 0) return;

        // use battery
        _gameController.equipment.BatteryCount_Update(-1);

        if (_data.heatLevel <= 0) return;

        if (Game_Controller.Percentage_Activated(_data.detectChance + _detectBonusRate) == false) return;

        if (_revealedHeatNum + 1 > _data.heatLevel) return;

        _sr.sprite = _gameController.data.BaggageSprite(_data.typeNum, _revealedHeatNum + 1);
        _revealedHeatNum++;
    }

    // for Cuff
    private void HeatLevel3_Arrest()
    {
        if (Equipment_Controller.currentEquipment != Equipment.cuff) return;

        Movement_Toggle(false);

        _ownerNPC.interaction.Collect_Baggage();
        _ownerNPC.movement.Leave();
        _gameController.checkPoints[_checkNum].Remove_Baggage(this);

        _ownerNPC.interaction.Arrest_Toggle(true);
    }
}
