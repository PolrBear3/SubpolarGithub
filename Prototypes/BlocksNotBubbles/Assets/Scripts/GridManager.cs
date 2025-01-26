using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Transform _GridCellsParent;
    [SerializeField] private Transform _launchCellsParent;

    [Header("")]
    [SerializeField] private GameObject _gridCellObject;

    [Header("")]
    [SerializeField] private Vector2 _gridSize;
    public Vector2 gridSize => _gridSize;

    [SerializeField] private Vector2 _startGridPos;


    private List<GridCell_Controller> _cellControllers = new();
    public List<GridCell_Controller> cellControllers => _cellControllers;

    private List<GridCell_Controller> _launchCells = new();
    public List<GridCell_Controller> launchCells => _launchCells;


    // MonoBehaviour
    private void Start()
    {
        Set_GridCells();
        Set_LaunchGridCells();
    }


    // Grid Cells
    private GridCell_Controller GridCell_Controller(Vector2 targetPos)
    {
        for (int i = 0; i < _cellControllers.Count; i++)
        {
            if (targetPos != _cellControllers[i].data.position) continue;
            return _cellControllers[i];
        }

        return null;
    }

    private void Set_GridCells()
    {
        for (int y = 0; y < (int)_gridSize.y; y++)
        {
            for (int x = 0; x < (int)_gridSize.x; x++)
            {
                Vector2 cellPosition = new(_startGridPos.x + (x * 0.625f), _startGridPos.y - (y * 0.625f));

                GameObject cellObject = Instantiate(_gridCellObject, cellPosition, Quaternion.identity, transform);
                cellObject.transform.SetParent(_GridCellsParent);

                GridCell_Controller cellController = cellObject.GetComponent<GridCell_Controller>();
                _cellControllers.Add(cellController);

                GridCell cellData = new(new Vector2(x, y));
                cellController.Set_Data(cellData);
            }
        }

        // set surrounding positions
        Vector2[] surroundingDirections = Main_Controller.instance.surroundDirections;

        for (int i = 0; i < _cellControllers.Count; i++)
        {
            for (int j = 0; j < surroundingDirections.Length; j++)
            {
                Vector2 targetPos = _cellControllers[i].data.position + surroundingDirections[j];

                if (GridCell_Controller(targetPos) == null) continue;

                _cellControllers[i].data.surroundingPos.Add(targetPos);
            }
        }
    }


    private void Set_LaunchGridCells()
    {
        Sprite launchCell = Main_Controller.instance.launcher.gridCellSprite;
        float height = _startGridPos.y * -1;

        for (int x = 0; x < (int)_gridSize.x; x++)
        {
            Vector2 cellPosition = new(_startGridPos.x + (x * 0.625f), height);

            GameObject cellObject = Instantiate(_gridCellObject, cellPosition, Quaternion.identity, transform);
            cellObject.transform.SetParent(_launchCellsParent);

            GridCell_Controller cellController = cellObject.GetComponent<GridCell_Controller>();
            _launchCells.Add(cellController);

            cellController.sr.sprite = launchCell;

            GridCell cellData = new(new Vector2(x, 0));
            cellController.Set_Data(cellData);
        }
    }


    // Blocks
    public Block_Controller Set_Block(Block blockData, int xPos)
    {
        if (xPos < -1 || xPos > _gridSize.x - 1) return null;

        for (int y = 0; y < (int)_gridSize.y; y++)
        {
            Vector2 targetPos = new(xPos, y);

            GridCell_Controller cellController = GridCell_Controller(targetPos);
            if (cellController == null) break;

            // check if cell has block
            GridCell cell = GridCell_Controller(targetPos).data;
            if (cell.occupied) continue;

            // instantiation
            Vector2 setPos = cellController.transform.position;

            GameObject setBlock = Instantiate(Main_Controller.instance.blockObject, setPos, Quaternion.identity);
            setBlock.transform.SetParent(cellController.transform);

            Block_Controller blockController = setBlock.GetComponent<Block_Controller>();

            // block update
            blockController.Set_Data(blockData);
            blockController.sr.sprite = blockData.blockType.setSprite;

            // cell update
            cell.Update_Block(blockController);
            cellController.sr.sprite = blockData.blockType.cellSprite;

            return blockController;
        }

        return null;
    }

    public void Destroy_BlockMatch(Vector2 startingPos)
    {
        GridCell_Controller startingCell = GridCell_Controller(startingPos);

        // check if starting position cell has block
        if (startingCell == null || startingCell.data.occupied == false) return;

        Block_Controller startingBlock = startingCell.data.occupiedBlock;

        List<GridCell_Controller> cellsToCheck = new();
        List<Block_Controller> blocksToDestroy = new();

        /*
        while (cellsToCheck.Count > 0)
        {

        }
        */
    }
}
