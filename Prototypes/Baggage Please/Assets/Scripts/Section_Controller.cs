using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section_Controller : MonoBehaviour
{
    [SerializeField] private Transform _waitPoint;
    public Transform waitPoint => _waitPoint;
}