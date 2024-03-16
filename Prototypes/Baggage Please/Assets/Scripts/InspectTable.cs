using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectTable : MonoBehaviour
{
    [SerializeField] private Transform _baggagePoint;
    public Transform baggagePoint => _baggagePoint;
}
