using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rhythm_HitBox : MonoBehaviour
{
    private PlayerInput _playerInput;

    private SpriteRenderer _spriteRenderer;

    [Header("")]
    [SerializeField] private List<Sprite> _transitionBoxes = new();

    [SerializeField] private Sprite _boxHitSprite;
    [SerializeField] private Sprite _boxUnHitSprite;

    [Header("")]
    [SerializeField] private SpriteRenderer _hitBox;
    [SerializeField] private GameObject _actionKeyText;

    private int _transitionNum;

    [Header("")]
    [SerializeField] private float _transitionTime;
    public float transitionTime => _transitionTime;

    private bool _boxHit;

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
        Main_Controller.Change_SpriteAlpha(_hitBox, 0f);
    }



    // InputSystem
    private void OnHit()
    {
        Hit_Box();
    }



    // HitBox Activation
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
            _boxHit = false;

            _spriteRenderer.sprite = _transitionBoxes[_transitionNum];

            // if hit ready, show hit key
            if (_transitionNum >= _transitionBoxes.Count - 1)
            {
                _actionKeyText.SetActive(true);
            }

            yield return new WaitForSeconds(_transitionTime);

            _transitionNum++;

            // reset
            if (_transitionNum >= _transitionBoxes.Count)
            {
                _actionKeyText.SetActive(false);
                _transitionNum = 0;
            }
        }
    }



    /// <summary>
    /// Signals the parent of this prefab
    /// </summary>
    private void Hit_Box()
    {
        if (_transitionNum >= _transitionBoxes.Count - 1)
        {
            if (_boxHit) return;
            if (!transform.parent.TryGetComponent(out ISignal parent)) return;

            _boxHit = true;

            Indicate_HitBox_Sprite(_boxHitSprite);
            parent.Signal();
        }
        else
        {
            Indicate_HitBox_Sprite(_boxUnHitSprite);
            Activate_HitBox();
        }
    }

    /// <summary>
    /// sets _hitBox to _boxHitSprite when hit on time, _boxUnHitSprite off time
    /// </summary>
    private void Indicate_HitBox_Sprite(Sprite _hitBoxSprite)
    {
        StartCoroutine(Indicate_HitBox_Sprite_Coroutine(_hitBoxSprite));
    }
    private IEnumerator Indicate_HitBox_Sprite_Coroutine(Sprite _hitBoxSprite)
    {
        _hitBox.sprite = _hitBoxSprite;
        Main_Controller.Change_SpriteAlpha(_hitBox, 1f);

        yield return new WaitForSeconds(_transitionTime);

        Main_Controller.Change_SpriteAlpha(_hitBox, 0f);
    }
}
