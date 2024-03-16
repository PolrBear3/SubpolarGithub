using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Baggage : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private SpriteRenderer _sr;
    private Game_Controller _gameController;

    [Header("")]
    [SerializeField] private List<Sprite> _baggageSprites = new();

    [Header("")]
    [SerializeField] private float _speed;

    private NPC_Controller _ownerNPC;
    public NPC_Controller ownerNPC => _ownerNPC;

    private Baggage_CheckPoint _detectedCheckPoint;

    private int _checkNum;
    public int checkNum => _checkNum;

    private bool _dragging;
    public bool dragging => _dragging;

    private bool _moveable;
    public bool moveable => _moveable;



    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _gameController = FindObjectOfType<Game_Controller>();
    }

    private void Start()
    {
        Set_Sprite();
    }

    private void Update()
    {
        if (_moveable == false) return;
        transform.Translate(_speed * Time.deltaTime * new Vector2(1, 0));
    }



    // EventSystems
    public void OnDrag(PointerEventData eventData)
    {
        _dragging = true;

        Movement_Toggle(false);

        transform.parent = _gameController.transform;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(mousePos.x, mousePos.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;

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
    private void Set_Sprite()
    {
        int spriteNum = Random.Range(0, _baggageSprites.Count);
        _sr.sprite = _baggageSprites[spriteNum];
    }

    public void Set_OwnerNPC(NPC_Controller owner)
    {
        _ownerNPC = owner;
    }
}
