using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Menu_Controller : MonoBehaviour
{
    private PlayerInput _playerInput;

    private Game_Controller _gameController;

    [SerializeField] private GameObject _title;
    [SerializeField] private GameObject _gameStartButton;

    [Header("")]
    [SerializeField] private GameObject _camera;
    [SerializeField] private RectTransform _equipmentPanel; // -500 -225
    [SerializeField] private RectTransform _scoreText; // 450 355

    [Header("")]
    [SerializeField] private float _transitionTime;

    // UnityEngine
    private void Awake()
    {
        _playerInput = gameObject.GetComponent<PlayerInput>();
        _gameController = FindObjectOfType<Game_Controller>();
    }

    private void Start()
    {
        _playerInput.enabled = false;

        _camera.transform.position = new Vector3(-10f, _camera.transform.position.y, -1f);
        _equipmentPanel.anchoredPosition = new(_equipmentPanel.anchoredPosition.x, -500f);
        _scoreText.anchoredPosition = new(_scoreText.anchoredPosition.x, 450f);
    }



    //
    public void Start_Game()
    {
        _gameController.Start_Game();

        _playerInput.enabled = true;

        _title.SetActive(false);
        _gameStartButton.SetActive(false);

        LeanTween.moveX(_camera, 0f, _transitionTime).setEaseInOutQuint();
        LeanTween.moveY(_equipmentPanel, -225f, _transitionTime).setEaseInOutQuint();
        LeanTween.moveY(_scoreText, 355f, _transitionTime).setEaseInOutQuint();
    }
}
