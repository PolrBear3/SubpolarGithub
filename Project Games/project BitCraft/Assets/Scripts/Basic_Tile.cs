using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Basic_Tile : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Data")]
    [SerializeField] private int tileNum;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
}
