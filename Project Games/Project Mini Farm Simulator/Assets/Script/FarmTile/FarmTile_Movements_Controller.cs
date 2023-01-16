using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmTile_Movements_Controller : MonoBehaviour
{
    [SerializeField] private FarmTile_Movement[] farmTiles;

    public void All_Start_Position()
    {
        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTiles[i].Start_Position();
        }
    }

    public void All_LeanTween_Start_Position(float delayTime)
    {
        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTiles[i].LeanTween_Start_Position(delayTime);
        }
    }
    public void All_LeanTween_Set_Position(float delayTime)
    {
        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTiles[i].LeanTween_Set_Position(delayTime);
        }
    }
}
