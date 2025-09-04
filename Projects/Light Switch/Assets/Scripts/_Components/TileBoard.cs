using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Vector2 _defaultBoardSize;
    [SerializeField] [Range(0, 10)] private float _gapValue;

    [Space(20)]
    [SerializeField] private Transform _setTileStartPoint;
    [SerializeField] private GameObject _tilePrefab;
    
    
    // MonoBehaviour
    private void Start()
    {
        Main_Controller.instance.invokeExecutionController.Subscribe_Action(Set_Tiles, 0);
    }

    private void OnDestroy()
    {
        Main_Controller.instance.invokeExecutionController.Unsubscribe_Action(Set_Tiles, 0);
    }


    // Main
    private void Set_Tiles()
    {
        Vector2 startPosition = _setTileStartPoint.position;
        
        for (int y = 0; y < (int)_defaultBoardSize.y; y++)
        {
            for (int x = 0; x < (int)_defaultBoardSize.x; x++)
            {
                Vector2 cellPosition = new(startPosition.x + (x * _gapValue), startPosition.y - (y * _gapValue));

                GameObject cellObject = Instantiate(_tilePrefab, cellPosition, Quaternion.identity, transform);
                cellObject.transform.SetParent(_setTileStartPoint);
            }
        }
    }
}
