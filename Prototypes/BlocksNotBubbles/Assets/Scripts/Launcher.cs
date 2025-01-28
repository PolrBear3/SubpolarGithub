using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Launcher : MonoBehaviour
{
    [Header("")]
    [SerializeField] private PlayerInput _playerInput;

    [Header("")]
    [SerializeField] private Sprite _gridCellSprite;
    public Sprite gridCellSprite => _gridCellSprite;


    [Header("")]
    [SerializeField][Range(0, 100)] private int _maxLoadAmount;
    public int maxLoadAmount => _maxLoadAmount;


    private int _currentXPos;
    public int currentXPos => _currentXPos;

    private List<Block> _loadedBlocks = new();
    public List<Block> loadedBlocks => _loadedBlocks;

    private GameObject _currentBlock;


    // private Coroutine _launchCoroutine;


    // MonoBehaviour
    private void Start()
    {
        GridManager manager = Main_Controller.instance.gridManager;

        _currentXPos = manager.launchCells.Count / 2;
        Update_CurrentPosition(0);

        // load block test
        Block_ScrObj[] allBlockTypes = Main_Controller.instance.blockTypes;

        for (int i = 0; i < _maxLoadAmount; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, allBlockTypes.Length);
            Load_Block(allBlockTypes[randIndex]);
        }

        Place_NextBlock();
    }


    // InputSystem
    private void OnMovement(InputValue value)
    {
        Vector2 direction = value.Get<Vector2>();

        Update_CurrentPosition((int)direction.x);
    }

    private void OnInteract()
    {
        Launch_Block();
    }


    // Current Blocks
    private void Load_Block(Block_ScrObj blockType)
    {
        if (_loadedBlocks.Count >= _maxLoadAmount) return;

        Block loadBlock = new(blockType);
        _loadedBlocks.Add(loadBlock);
    }

    private void Set_Block(Block_ScrObj blockType)
    {
        if (_loadedBlocks.Count >= _maxLoadAmount) return;

        Block setBlock = new(blockType);
        _loadedBlocks.Insert(0, setBlock);
    }


    private void Place_CurrentBlock(Block placeBlockData)
    {
        GameObject blockObject = Main_Controller.instance.blockObject;
        GameObject placeObject = Instantiate(blockObject, transform.position, quaternion.identity);

        placeObject.transform.SetParent(transform);

        Block_Controller blockController = placeObject.GetComponent<Block_Controller>();

        blockController.Set_Data(placeBlockData);
        blockController.Update_CurrentSprite(false);

        _currentBlock = placeObject;
    }

    private void Place_NextBlock()
    {
        if (_loadedBlocks.Count <= 0) return;

        Place_CurrentBlock(_loadedBlocks[0]);
    }


    // Control
    private void Update_CurrentPosition(int direction)
    {
        GridManager manager = Main_Controller.instance.gridManager;

        manager.launchCells[_currentXPos].Toggle_Transparency(false);

        _currentXPos = Mathf.Clamp(_currentXPos + direction, 0, manager.launchCells.Count - 1);
        transform.position = manager.launchCells[_currentXPos].transform.position;

        manager.launchCells[_currentXPos].Toggle_Transparency(true);
    }

    private void Launch_Block()
    {
        if (_loadedBlocks.Count <= 0) return;

        GridManager manager = Main_Controller.instance.gridManager;

        Destroy(_currentBlock);
        manager.Set_Block(_loadedBlocks[0], _currentXPos);

        _loadedBlocks.RemoveAt(0);
        Place_NextBlock();

        manager.Process_MatchingAndGaps();
    }
    private IEnumerator Launch_Block_Coroutine()
    {
        // _launchCoroutine = null;
        yield break;
    }
}
