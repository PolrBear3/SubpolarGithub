using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Tutorial_Controller : MonoBehaviour, ISaveLoadable
{
    private PlayerInput _input;
    private PlayerInput _currentDisableInput;

    [Header("")]
    [SerializeField] private RectTransform _background;
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private Animator _gifAnimator;
    [SerializeField] private TextMeshProUGUI _explanationText;
    [SerializeField] private TextMeshProUGUI _pageNumText;

    private List<Tutorial_ScrObj> _shownTutorials = new();

    private Tutorial_ScrObj _currentTutorial;
    private int _currentArrayNum;

    [Header("Background Value")]
    [SerializeField] private float _transparencyValue;
    [SerializeField] private float _transitionTime;

    [Header("")]
    [SerializeField] private bool _tutorialSystemActive;

    [Header("")]
    [SerializeField] private List<Tutorial_ScrObj> _allTutorials = new();



    // UnityEngine
    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();
    }



    // InputSystem
    private void OnOption1()
    {
        _currentArrayNum--;

        if (_currentArrayNum < 0) _currentArrayNum = _currentTutorial.explanation.Count - 1;

        // set explanation
        _explanationText.text = _currentTutorial.explanation[_currentArrayNum];

        // page number
        _pageNumText.text = (_currentArrayNum + 1 + "/" + _currentTutorial.explanation.Count).ToString();
    }

    private void OnOption2()
    {
        _currentArrayNum++;

        if (_currentArrayNum > _currentTutorial.explanation.Count - 1) _currentArrayNum = 0;

        // set explanation
        _explanationText.text = _currentTutorial.explanation[_currentArrayNum];

        // page number
        _pageNumText.text = (_currentArrayNum + 1 + "/" + _currentTutorial.explanation.Count).ToString();
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
        List<int> tutorialIDs = new();

        for (int i = 0; i < _shownTutorials.Count; i++)
        {
            if (_shownTutorials[i] == null) continue;

            tutorialIDs.Add(_shownTutorials[i].id);
        }

        ES3.Save("Tutorial_Controller/tutorialIDs", tutorialIDs);
    }

    public void Load_Data()
    {
        List<int> tutorialIDs = new();

        tutorialIDs = ES3.Load("Tutorial_Controller/tutorialIDs", tutorialIDs);

        for (int i = 0; i < tutorialIDs.Count; i++)
        {
            _shownTutorials.Add(Tutorial_ScrObj(tutorialIDs[i]));
        }
    }



    /// <returns>
    /// Tutorial_ScrObj by id
    /// </returns>
    public Tutorial_ScrObj Tutorial_ScrObj(int id)
    {
        for (int i = 0; i < _allTutorials.Count; i++)
        {
            if (id != _allTutorials[i].id) continue;
            return _allTutorials[i];
        }

        return null;
    }



    // Check
    public bool Tutorial_Shown(Tutorial_ScrObj tutorial)
    {
        if (_tutorialSystemActive) return true;

        for (int i = 0; i < _shownTutorials.Count; i++)
        {
            if (tutorial != _shownTutorials[i]) continue;
            return true;
        }

        return false;
    }



    // Main Control
    public void Show_Tutorial(PlayerInput disableInput, Tutorial_ScrObj tutorial)
    {
        if (Tutorial_Shown(tutorial) == false)
        {
            _shownTutorials.Add(tutorial);
        }

        // input system control
        _input.enabled = true;
        Main_Controller.gamePaused = true;

        if (disableInput != null)
        {
            _currentDisableInput = disableInput;
            _currentDisableInput.enabled = false;
        }

        // background effect
        LeanTween.alpha(_background, _transparencyValue * 0.01f, _transitionTime);

        // set data
        _currentArrayNum = 0;
        _currentTutorial = tutorial;

        // set gif
        _gifAnimator.runtimeAnimatorController = _currentTutorial.gifAnimator;

        // set explanation
        _explanationText.text = _currentTutorial.explanation[_currentArrayNum];

        // page number
        _pageNumText.text = (_currentArrayNum + 1 + "/" + _currentTutorial.explanation.Count).ToString();

        // panel on
        _tutorialPanel.SetActive(true);
    }

    public void Hide_Tutorial()
    {
        _input.enabled = false;
        Main_Controller.gamePaused = false;

        if (_currentDisableInput != null)
        {
            _currentDisableInput.enabled = true;
            _currentDisableInput = null;
        }

        // background
        LeanTween.alpha(_background, 0f, _transitionTime);

        // panel off
        _tutorialPanel.SetActive(false);
    }
}