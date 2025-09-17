using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Controller : MonoBehaviour
{
    public static Game_Controller instance;
    
    [Space(20)]
    [SerializeField] private Cursor _cursor;
    public Cursor cursor => _cursor;
    
    [SerializeField] private TableTop _tableTop;
    public TableTop tableTop => _tableTop;
    
    
    // MonoBehaviour
    private void Awake()
    {
        instance = this;
    }
}
