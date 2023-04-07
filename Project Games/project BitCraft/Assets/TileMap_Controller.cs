using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap_Controller : MonoBehaviour
{
    [SerializeField] private GameObject basicTile;
    private List<Basic_Tile> tiles = new List<Basic_Tile>();

    [Header("Data")]
    [SerializeField] private int maxTileNum;

    private void Awake()
    {
        Set_25_Tiles();
    }

    public void Set_25_Tiles()
    {
        int positionX = -2;
        int positionY = 2;

        for (int i = 0; i < maxTileNum; i++)
        {
            // set position
            Vector2 position = new Vector2(positionX, positionY);

            // instantiate
            GameObject tile = Instantiate(basicTile, position, Quaternion.identity);

            // set inside tile map as child

            // add to tiles list
            Basic_Tile tileData = tile.GetComponent<Basic_Tile>();
            tiles.Add(tileData);

            // update data
            tileData.Set_Data(positionX, positionY);
            
            // next position
            positionX++;

            // column change
            if (positionX == 3)
            {
                positionX = -2;
                positionY--;
            }
        }
    }

}
