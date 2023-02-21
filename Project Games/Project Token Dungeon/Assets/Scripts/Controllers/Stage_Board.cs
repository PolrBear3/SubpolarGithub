using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stage_Board_Data
{
    public int maxRowNum;
    public int maxColumnNum;
}

public class Stage_Board : MonoBehaviour
{
    [SerializeField] private Stage_Board_Data data;

    public List<Stage_Tile> tiles = new List<Stage_Tile>();
    public Stage_Tile startingTile;

    public void Set_Tile_Data()
    {
        int rowNum = 0;
        int columnNum = 0;
        
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].Set_Data(rowNum, columnNum);

            columnNum++;

            if (columnNum != data.maxColumnNum) continue;

            columnNum = 0;
            rowNum++;
        }
    }
    public void Set_PlayerPiece(GameObject playerPiece)
    {
        playerPiece.transform.parent = startingTile.groundPoint.transform;
        playerPiece.transform.localPosition = Vector2.zero;
    }
}
