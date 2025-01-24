using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Controller : MonoBehaviour
{
    [Header("")]
    [SerializeField] private GridManager _gridManager;
    public GridManager gridManager => _gridManager;

    [SerializeField] private Launcher _launcher;
    public Launcher launcher => _launcher;
}
