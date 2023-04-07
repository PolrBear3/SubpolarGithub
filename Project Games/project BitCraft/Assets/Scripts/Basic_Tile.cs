using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Basic_Tile : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Data")]
    [SerializeField] private int tileNumX;
    [SerializeField] private int tileNumY;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Set_Data(int x, int y)
    {
        tileNumX = x;
        tileNumY = y;
    }
}
