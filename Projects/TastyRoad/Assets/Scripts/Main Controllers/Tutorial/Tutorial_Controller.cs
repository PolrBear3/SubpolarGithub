using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Tutorial_Controller : MonoBehaviour, ISaveLoadable
{
    private PlayerInput _input;

    [SerializeField] private RectTransform _background;
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private Animator _gifAnimator;
    [SerializeField] private TextMeshProUGUI _explanationText;

    private List<Tutorial_ScrObj> _shownTutorials = new();

    private Tutorial_ScrObj _currentTutorial;
    private int _currentArrayNum;

    [Header("Background Value")]
    [SerializeField] private float _transparencyValue;
    [SerializeField] private float _transitionTime;



    // UnityEngine
    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();
    }

    private void Start()
    {
        Hide_Tutorial();
    }



    // InputSystem
    private void OnOption1()
    {
        _currentArrayNum--;

        if (_currentArrayNum < 0) _currentArrayNum = _currentTutorial.explanation.Count - 1;

        // set explanation
        _explanationText.text = _currentTutorial.explanation[_currentArrayNum];
    }

    private void OnOption2()
    {
        _currentArrayNum++;

        if (_currentArrayNum > _currentTutorial.explanation.Count - 1) _currentArrayNum = 0;

        // set explanation
        _explanationText.text = _currentTutorial.explanation[_currentArrayNum];
    }

    private void OnSelect()
    {
        Hide_Tutorial();
    }

    private void OnExit()
    {
        Hide_Tutorial();
    }



    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("Tutorial_Controller_shownTutorials", _shownTutorials);
    }

    public void Load_Data()
    {
        _shownTutorials = ES3.Load("Tutorial_Controller_shownTutorials", _shownTutorials);
    }



    // Check
    public bool Tutorial_Shown(Tutorial_ScrObj tutorial)
    {
        for (int i = 0; i < _shownTutorials.Count; i++)
        {
            if (_shownTutorials[i] != tutorial) continue;
            return true;
        }
        return false;
    }



    // Main Control
    public void Show_Tutorial(Tutorial_ScrObj tutorial)
    {
        _input.enabled = true;
        Main_Controller.gamePaused = true;

        _shownTutorials.Add(tutorial);

        // background effect
        LeanTween.alpha(_background, _transparencyValue * 0.01f, _transitionTime);

        // set data
        _currentArrayNum = 0;
        _currentTutorial = tutorial;

        // set gif
        _gifAnimator.runtimeAnimatorController = _currentTutorial.gifAnimator;

        // set explanation
        _explanationText.text = _currentTutorial.explanation[_currentArrayNum];

        // panel on
        _tutorialPanel.SetActive(true);
    }

    public void Hide_Tutorial()
    {
        _input.enabled = false;
        Main_Controller.gamePaused = false;

        // background
        LeanTween.alpha(_background, 0f, _transitionTime);

        // panel off
        _tutorialPanel.SetActive(false);
    }
}