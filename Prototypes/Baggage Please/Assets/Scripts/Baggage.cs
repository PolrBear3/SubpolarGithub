using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Baggage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private SpriteRenderer _sr;

    private Game_Controller _gameController;

    [Header("")]
    [SerializeField] private List<Sprite> _baggageSprites = new();

    [SerializeField] private Transform _dropPoint;
    public Transform dropPoint => _dropPoint;

    [Header("")]
    [SerializeField] private float _speed;

    private bool _dragging;
    public bool dragging => _dragging;

    private bool _moveable;
    public bool moveable => _moveable;

    private bool _droppable;
    public bool droppable => _droppable;

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

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_droppable == false) return;
        if (!collision.TryGetComponent(out Baggage_DropPoint point)) return;
        _dropPoint = point.baggagePosition;
    }

    // EventSystems
    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        _dragging = true;

        transform.parent = _gameController.transform;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(mousePos.x, mousePos.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;

        transform.parent = _dropPoint;
        transform.localPosition = Vector2.zero;
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

    public void Droppable_Toggle(bool toggleOn)
    {
        _droppable = toggleOn;
    }

    //
    private void Set_Sprite()
    {
        int spriteNum = Random.Range(0, _baggageSprites.Count);
        _sr.sprite = _baggageSprites[spriteNum];
    }

    public void Set_DropPoint(Transform dropPoint)
    {
        _dropPoint = dropPoint;
        transform.parent = dropPoint;

        transform.localPosition = Vector2.zero;
    }
}
