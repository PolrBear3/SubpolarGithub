using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmTile_Movement : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private LeanTweenType tweenType;

    [SerializeField] private float startPosition;
    [SerializeField] private float setPosition;
    [SerializeField] private float speed;
    [SerializeField] private float delayTime;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}
