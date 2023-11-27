using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sr;

    private Game_Controller _gameController;
    private Player_Controller _playerController;

    private Food _currentFood;

    [SerializeField] private Sprite _activeSprite;
    [SerializeField] private Sprite _inactiveSprite;

    private Coroutine _testCoroutine;
    [SerializeField] private int _testHeatLevel;

    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();

        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
    }

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller playerController)) return;
        _playerController = playerController;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player_Controller playerController)) return;
        _playerController = null;
    }

    // IInteractable
    public void Interact()
    {
        if (_playerController == null) return;
        if (!_playerController.playerInteraction.Is_Closest_Interactable(gameObject)) return;

        Swap_Food();
        Sprite_Update();

        //
        if (_testCoroutine != null) StopCoroutine(_testCoroutine);

        if (_currentFood == null) return;

        _testCoroutine = StartCoroutine(Heat_Food());
        //
    }

    // Custom
    private void Swap_Food()
    {
        Player_Interaction player = _playerController.playerInteraction;
        Food playerFood = player.currentFood;

        player.Set_CurrentFood(_currentFood);
        _currentFood = playerFood;
    }
    private void Sprite_Update()
    {
        // active
        if (_currentFood != null) _sr.sprite = _activeSprite;

        // inactive
        else _sr.sprite = _inactiveSprite;
    }

    private IEnumerator Heat_Food()
    {
        while (_currentFood != null)
        {
            yield return new WaitForSeconds(1f);
            _testHeatLevel++;
        }
    }
}
