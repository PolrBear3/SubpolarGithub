using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Light : MonoBehaviour
{
    private BoxCollider2D _boxCollider;

    private Player_Movement _player;

    private bool _wallDetected;
    private bool _playerDetected;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out BoxCollider2D bc)) _boxCollider = bc;
    }
    private void Update()
    {
        if (!_wallDetected && _playerDetected)
        {
            if (_player != null)
            {
                _player.Die();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Wall wall))
        {
            _wallDetected = true;
            LeanTween.alpha(gameObject, 0f, 0.5f);
        }

        if (collision.TryGetComponent(out Player_Movement player))
        {
            _player = player;
            _playerDetected = true;

            if (_wallDetected) return;
            player.Die();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Wall wall))
        {
            _wallDetected = false;
            LeanTween.alpha(gameObject, 0.2f, 0.5f);
        }

        if (collision.TryGetComponent(out Player_Movement player))
        {
            _playerDetected = false;
        }
    }
}
