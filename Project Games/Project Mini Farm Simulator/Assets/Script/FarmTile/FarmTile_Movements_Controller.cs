using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmTile_Movements_Controller : MonoBehaviour
{
    [SerializeField] private FarmTile_Movement[] farmTiles;

    private void Start()
    {
        All_Start_Position();
    }

    private void All_Start_Position()
    {
        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTiles[i].Start_Position();
        }
    }
}
