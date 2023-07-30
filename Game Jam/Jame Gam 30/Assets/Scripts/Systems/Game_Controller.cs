using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject _player;

    [SerializeField] private Player_Movement _currentPlayer;
    public Player_Movement currentPlayer { get => _currentPlayer; set => _currentPlayer = value; }

    [Header("Level")]
    [SerializeField] private List<GameObject> _allLevels = new List<GameObject>();
    public List<GameObject> allLevels { get => _allLevels; set => _allLevels = value; }

    [SerializeField] private Level_Controller _currentLevel;
    public Level_Controller currentLevel { get => _currentLevel; set => _currentLevel = value; }

    //
    private void Start()
    {
        Set_Level(0);
        Set_Player();
    }

    // Game Set
    private void Set_Level(int levelNum)
    {
        _currentLevel = null;
        GameObject level = Instantiate(allLevels[levelNum]);
        if (level.TryGetComponent(out Level_Controller levelController)) _currentLevel = levelController;
        _currentLevel.gameController = this;
    }
    private void Set_Player()
    {
        _currentPlayer = null;
        GameObject player = Instantiate(_player);
        if (player.TryGetComponent(out Player_Movement currentPlayer)) _currentPlayer = currentPlayer;
        _currentPlayer.gameController = this;
    }
}
