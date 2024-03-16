using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Camera_Controller : MonoBehaviour
{
    private Game_Controller _gameController;

    [SerializeField] private GameObject _camera;

    private int _currentSectionNum;

    [Header("")]
    [SerializeField] private float _transitionTime;

    // UnityEngine
    private void Awake()
    {
        _gameController = gameObject.GetComponent<Game_Controller>();
    }

    // InputSystem
    public void OnAction1()
    {
        _currentSectionNum--;

        if (_currentSectionNum < 0)
        {
            _currentSectionNum = 0;
            return;
        }

        LeanTween.moveX(_camera, _currentSectionNum * 10, _transitionTime).setEaseInOutQuint();
    }

    public void OnAction2()
    {
        _currentSectionNum++;

        if (_currentSectionNum > _gameController.sections.Count - 1)
        {
            _currentSectionNum = _gameController.sections.Count - 1;
            return;
        }

        LeanTween.moveX(_camera, _currentSectionNum * 10, _transitionTime).setEaseInOutQuint();
    }
}
