using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Controller : MonoBehaviour
{
    private Game_Controller _gameController;
    public Game_Controller gameController { get => _gameController; set => _gameController = value; }

    [SerializeField] private Time_Controller _timeController;
    public Time_Controller timeController { get => _timeController; set => _timeController = value; }

    private Gold_Gear _goldGear;
    public Gold_Gear goldGear { get => _goldGear; set => _goldGear = value; }

    [Header("Set Data")]
    [SerializeField] private List<Tile> _allTiles = new List<Tile>();
    public List<Tile> allTiles { get => _allTiles; set => _allTiles = value; }

    [SerializeField] private int levelSizeX;
    [SerializeField] private int levelSizeY;

    [SerializeField] private Transform _playerStartTile;
    public Transform playerStartTile { get => _playerStartTile; set => _playerStartTile = value; }

    private void Awake()
    {
        Set_AllTiles_Data();
        AllGears_Spin_Activation_Check();

        _timeController.Start_Game();
    }

    // Set
    private void Set_AllTiles_Data()
    {
        int xNum = 0;
        int yNum = 0;

        for (int i = 0; i < _allTiles.Count; i++)
        {
            _allTiles[i].Set_Data(this, xNum, yNum);
            xNum++;

            if (xNum <= levelSizeX) continue;
            yNum++;
            xNum = 0;
        }
    }

    // Get
    public Tile Get_Tile(int xNum, int yNum)
    {
        for (int i = 0; i < _allTiles.Count; i++)
        {
            if (_allTiles[i].xPosition != xNum) continue;
            if (_allTiles[i].yPosition != yNum) continue;
            return _allTiles[i];
        }
        return null;
    }
    public List<Tile> Surrounding_Tiles(Tile targetTile)
    {
        List<Tile> surroundingTiles = new List<Tile>();

        // top
        surroundingTiles.Add(Get_Tile(targetTile.xPosition, targetTile.yPosition - 1));
        // bottom
        surroundingTiles.Add(Get_Tile(targetTile.xPosition, targetTile.yPosition + 1));
        // left
        surroundingTiles.Add(Get_Tile(targetTile.xPosition - 1, targetTile.yPosition));
        // right
        surroundingTiles.Add(Get_Tile(targetTile.xPosition + 1, targetTile.yPosition));

        for (int i = surroundingTiles.Count - 1; i >= 0; i--)
        {
            if (surroundingTiles[i] != null) continue;
            surroundingTiles.RemoveAt(i);
        }

        return surroundingTiles;
    }

    // All Gears Update
    public void AllGears_Spin_Activation_Check()
    {
        // refresh calculation
        for (int i = 0; i < _allTiles.Count; i++)
        {
            if (_allTiles[i].currentGear == null) continue;
            _allTiles[i].currentGear.spinInActive = false;
            _allTiles[i].currentGear.Spin_Activation_Check(true);
        }
        // calculate
        for (int i = 0; i < _allTiles.Count; i++)
        {
            if (_allTiles[i].currentGear == null) continue;
            _allTiles[i].currentGear.Spin_Activation_Check(false);
        }

        // object gear
        for (int i = 0; i < _allTiles.Count; i++)
        {
            if (_allTiles[i].currentGear == null) continue;
            if (_allTiles[i].currentGear.objectGear == null) continue;
            _allTiles[i].currentGear.objectGear.Activate_Object();
        }

        // gold gear update
        if (_goldGear == null) return;
        _goldGear.SpinReverse_Check();
    }

    // Level Controller
    IEnumerator Next_Level_Delay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        _gameController.currentLevelNum++;
        _gameController.Save_Level();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Next_Level(float delayTime)
    {
        StartCoroutine(Next_Level_Delay(delayTime));
    }

    IEnumerator Reload_Level_Delay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        _gameController.Save_Level();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Reload_Level(float delayTime)
    {
        StartCoroutine(Reload_Level_Delay(delayTime));
    }
}
