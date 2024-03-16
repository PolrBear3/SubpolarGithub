using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Baggage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private SpriteRenderer _sr;

    public delegate void Event();
    public event Event PointerClick_Event;

    [Header("")]
    [SerializeField] private List<Sprite> _baggageSprites = new();

    private Transform _baggageLocation;
    public Transform baggageLocation => _baggageLocation;

    private Baggage_DropPoint _detectedDropPoint;
    public Baggage_DropPoint detectedDropPoint => _detectedDropPoint;

    [Header("")]
    [SerializeField] private float _speed;

    private bool _moveable;
    public bool moveable => _moveable;

    private bool _dragging;
    public bool dragging => _dragging;

    private bool _droppable;
    public bool droppable => _droppable;

    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
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

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage_DropPoint point)) return;
        _detectedDropPoint = point;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage_DropPoint point)) return;
        _detectedDropPoint = null;
    }

    // Event Trigger
    public void PointerClick()
    {
        PointerClick_Event?.Invoke();
    }

    // EventSystems
    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_moveable) return;

        _dragging = true;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(mousePos.x, mousePos.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_moveable) return;

        _dragging = false;

        if (_detectedDropPoint != null && _droppable)
        {
            _detectedDropPoint.RePosition_DroppedBaggages();
        }
        else
        {
            transform.localPosition = Vector2.zero;
        }
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

    public void Droppable_Toggle(bool toggleOn)
    {
        _droppable = toggleOn;
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

    public void Set_Location(Transform location)
    {
        _baggageLocation = location;
    }
}
