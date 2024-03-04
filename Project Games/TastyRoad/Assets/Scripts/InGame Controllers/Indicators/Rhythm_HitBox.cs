using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rhythm_HitBox : MonoBehaviour
{
    private PlayerInput _playerInput;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private List<Sprite> _hitBoxes = new();
    [SerializeField] private GameObject _actionKeyText;

    private int _transitionNum;

    [Header("")]
    [SerializeField] private float _transitionTime;
    public float transitionTime => _transitionTime;

    private Coroutine _hitBoxCoroutine;




    // UnityEngine
    private void Awake()
    {
        _playerInput = gameObject.GetComponent<PlayerInput>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Deactivate_HitBox();
    }



    // InputSystem
    private void OnAction1()
    {
        Hit_Box();
    }

    private void OnAction2()
    {
        Hit_Box();
    }



    //
    public void Deactivate_HitBox()
    {
        _actionKeyText.SetActive(false);

        _playerInput.enabled = false;
        _spriteRenderer.color = Color.clear;
        if (_hitBoxCoroutine != null) StopCoroutine(_hitBoxCoroutine);
    }

    public void Activate_HitBox()
    {
        _actionKeyText.SetActive(false);

        if (_hitBoxCoroutine != null) StopCoroutine(_hitBoxCoroutine);

        _transitionNum = 0;

        _playerInput.enabled = true;
        _spriteRenderer.color = Color.white;
        _hitBoxCoroutine = StartCoroutine(Activate_Hitbox_Coroutine());
    }
    private IEnumerator Activate_Hitbox_Coroutine()
    {
        while (true)
        {
            _spriteRenderer.sprite = _hitBoxes[_transitionNum];

            // if hit ready, show hit key
            if (_transitionNum >= _hitBoxes.Count - 1) _actionKeyText.SetActive(true);

            yield return new WaitForSeconds(_transitionTime);

            _transitionNum++;

            // reset
            if (_transitionNum >= _hitBoxes.Count)
            {
                _actionKeyText.SetActive(false);
                _transitionNum = 0;
            }
        }
    }



    /// <summary>
    /// Signals only the parent of this object. Move this object as secondary child of parent.
    /// </summary>
    private void Hit_Box()
    {
        if (_transitionNum >= _hitBoxes.Count - 1)
        {
            if (!transform.parent.TryGetComponent(out ISignal parent)) return;
            parent.Signal();
        }
        else
        {
            Activate_HitBox();
        }
    }
}
