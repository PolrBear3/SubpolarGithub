using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField] private MainController _main;
    [SerializeField] private GlobalVolume_Controller _postProcessing;

    [Header("")]
    [SerializeField] private RectTransform _fadePanel;
    [SerializeField] private GameObject _mainMenuButtons;

    [Header("Pause Menu")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private TextMeshProUGUI _pauseText;
    [SerializeField] private TextMeshProUGUI _returnText;

    private bool _isContinuePlay;
    public bool isContinuePlay => _isContinuePlay;

    [Header("Tutorial Menu")]
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private Animator _tutorialAnimator;
    [SerializeField] private TextMeshProUGUI _tutorialText;
    [SerializeField] private GIF_ScrObj[] _tutorialGIFs;

    private int _tutorialNum;

    [Header("Combo Menu")]
    [SerializeField] private GameObject _comboPanel;
    [SerializeField] private Animator _comboAnimator;
    [SerializeField] private TextMeshProUGUI _comboText;
    [SerializeField] private GIF_ScrObj[] _comboGIFs;

    private int _comboNum;

    [Header("")]
    [SerializeField] private float _transitionTime;


    // MonoBehaviour
    private void Start()
    {
        // fade panel animation
        LeanTween.alpha(_fadePanel, 1f, 0f);
        LeanTween.alpha(_fadePanel, 0f, _transitionTime);
    }


    // Main
    public void MainMenu()
    {
        // go to main menu scene
        StartCoroutine(MainMenu_Coroutine());
    }
    private IEnumerator MainMenu_Coroutine()
    {
        Close_PauseMenu();

        // fade panel animation
        LeanTween.alpha(_fadePanel, 1f, _transitionTime);

        // wait until fade animation ends
        yield return new WaitForSeconds(_transitionTime);

        // load game scene
        SceneManager.LoadScene(0);
    }

    public void PlayGame()
    {
        // reload scene
        StartCoroutine(PlayGame_Coroutine());
    }
    private IEnumerator PlayGame_Coroutine()
    {
        Close_PauseMenu();

        // fade panel animation
        LeanTween.alpha(_fadePanel, 1f, _transitionTime);

        // wait until fade animation ends
        yield return new WaitForSeconds(_transitionTime);

        // load game scene
        SceneManager.LoadScene(1);
    }

    public void EndGame()
    {
        Open_PauseMenu();

        // update text
        _pauseText.fontSize = 35f;
        _pauseText.text = "YOU HAVE REACHED\n+" + _main.overallPopulationGoal + " <sprite=7> ON TURN " + _main.gameData.turnCount;

        _returnText.text = "CONTINUE PLAYING";
    }


    // Pause Menu
    public void Open_PauseMenu()
    {
        if (_main.gameData.overallPopulation >= _main.overallPopulationGoal)
        {
            _isContinuePlay = true;
        }

        // global volume post processing control
        /*
        _postProcessing.Blur_GameScreen_Toggle(true, _transitionTime);
        _postProcessing.Darken_GameScreen_Toggle(true, _transitionTime);
        */

        // panel on
        _pauseMenu.SetActive(true);

        // panel tilt animation
        _pausePanel.transform.Rotate(0f, 0f, -5f);
        LeanTween.rotateZ(_pausePanel, 0f, _transitionTime).setEase(LeanTweenType.easeOutElastic);
    }

    public void Close_PauseMenu()
    {
        // global volume post processing control
        /*
        _postProcessing.Blur_GameScreen_Toggle(false, _transitionTime);
        _postProcessing.Darken_GameScreen_Toggle(false, _transitionTime);
        */

        // panel off
        _pauseMenu.SetActive(false);
    }


    // Tutorial Menu
    public void Open_TutorialMenu()
    {
        _mainMenuButtons.SetActive(false);
        _tutorialPanel.SetActive(true);

        _tutorialNum = 0;
        Control_TutorialMenu(0);

        // panel tilt animation
        _tutorialPanel.transform.Rotate(0f, 0f, -10f);
        LeanTween.rotateZ(_tutorialPanel, 0f, _transitionTime).setEase(LeanTweenType.easeOutElastic);
    }

    public void Close_TutorialMenu()
    {
        _tutorialPanel.SetActive(false);
        _mainMenuButtons.SetActive(true);
    }

    public void Control_TutorialMenu(int controlNum)
    {
        _tutorialNum += controlNum;

        if (_tutorialNum > _tutorialGIFs.Length - 1) _tutorialNum = 0;
        else if (_tutorialNum < 0) _tutorialNum = _tutorialGIFs.Length - 1;

        // gif
        _tutorialAnimator.runtimeAnimatorController = _tutorialGIFs[_tutorialNum].animOverrider;
        _tutorialAnimator.Play("GIFAnimator_play");

        // description
        _tutorialText.text = _tutorialGIFs[_tutorialNum].description;
    }


    // Combo Menu
    public void Open_ComboMenu()
    {
        _mainMenuButtons.SetActive(false);
        _comboPanel.SetActive(true);

        _comboNum = 0;
        Control_ComboMenu(0);

        // panel tilt animation
        _comboPanel.transform.Rotate(0f, 0f, -10f);
        LeanTween.rotateZ(_comboPanel, 0f, _transitionTime).setEase(LeanTweenType.easeOutElastic);
    }

    public void Close_ComboMenu()
    {
        _comboPanel.SetActive(false);
        _mainMenuButtons.SetActive(true);
    }

    public void Control_ComboMenu(int controlNum)
    {
        _comboNum += controlNum;

        if (_comboNum > _comboGIFs.Length - 1) _comboNum = 0;
        else if (_comboNum < 0) _comboNum = _comboGIFs.Length - 1;

        // gif
        _comboAnimator.runtimeAnimatorController = _comboGIFs[_comboNum].animOverrider;
        _comboAnimator.Play("GIFAnimator_play");

        // description
        _comboText.text = _comboGIFs[_comboNum].description;
    }
}
