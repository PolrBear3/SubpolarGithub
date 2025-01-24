using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Main_Controller _mainController;

    [Header("")]
    [SerializeField] private GameObject _gridCellObject;

    [Header("")]
    [SerializeField] private Vector2 _gridSize;
    public Vector2 gridSize => _gridSize;

    [SerializeField] private Vector2 _startGridPos;


    private List<GridCell_Controller> _cellControllers = new();
    public List<GridCell_Controller> cellControllers => _cellControllers;


    // MonoBehaviour
    private void Start()
    {
        Set_GridCells();
    }


    // Grid Cells
    private void Set_GridCells()
    {
        for (int y = 0; y < (int)_gridSize.y; y++)
        {
            for (int x = 0; x < (int)_gridSize.x; x++)
            {
                Vector2 cellPosition = new(_startGridPos.x + (x * 0.625f), _startGridPos.y - (y * 0.625f));

                GameObject cellObject = Instantiate(_gridCellObject, cellPosition, Quaternion.identity, transform);
                GridCell_Controller cellController = cellObject.GetComponent<GridCell_Controller>();

                _cellControllers.Add(cellController);

                GridCell cellData = new(x, y);
                cellController.Set_Data(cellData);
            }
        }
    }
}
