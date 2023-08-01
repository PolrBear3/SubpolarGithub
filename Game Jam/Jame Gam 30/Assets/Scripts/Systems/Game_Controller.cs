using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject _player;

    private Player_Movement _currentPlayer;
    public Player_Movement currentPlayer { get => _currentPlayer; set => _currentPlayer = value; }

    [Header("Level")]
    [SerializeField] private List<GameObject> _allLevels = new List<GameObject>();
    public List<GameObject> allLevels { get => _allLevels; set => _allLevels = value; }

    private Level_Controller _currentLevel;
    public Level_Controller currentLevel { get => _currentLevel; set => _currentLevel = value; }

    private int _currentLevelNum;
    public int currentLevelNum { get => _currentLevelNum; set => _currentLevelNum = value; }

    [Header("Extra")]
    [SerializeField] private GameObject _openingCurtain;
    [SerializeField] private GameObject _deathCurtain;

    //
    private void Start()
    {
        Load_Level();
    }
    private void OnApplicationQuit()
    {
        Save_Level();
    }

    // Curtain Control
    public void Activate_OpeningCurtain()
    {
        LeanTween.alpha(_openingCurtain, 0f, _currentLevel.timeController.transitionTime);
    }
    public void Acivate_DeathCuratin()
    {
        LeanTween.alpha(_deathCurtain, 1f, _currentLevel.timeController.transitionTime);
    }

    // Game Set
    private void Load_Level()
    {
        _currentLevelNum = ES3.Load("_currentLevelNum", _currentLevelNum);
        if (_currentLevelNum >= _allLevels.Count - 1) _currentLevelNum = _allLevels.Count - 1;
        Set_Level(_currentLevelNum);

        Activate_OpeningCurtain();
    }
    public void Save_Level()
    {
        ES3.Save("_currentLevelNum", _currentLevelNum);
    }

    public void Set_Level(int levelNum)
    {
        _currentLevel = null;
        GameObject level = Instantiate(allLevels[levelNum]);
        if (level.TryGetComponent(out Level_Controller levelController)) _currentLevel = levelController;
        _currentLevel.gameController = this;

        Set_Player();
    }
    private void Set_Player()
    {
        _currentPlayer = null;
        GameObject player = Instantiate(_player);
        if (player.TryGetComponent(out Player_Movement currentPlayer)) _currentPlayer = currentPlayer;
        _currentPlayer.gameController = this;

        player.transform.position = _currentLevel.playerStartTile.position;
    }
}
