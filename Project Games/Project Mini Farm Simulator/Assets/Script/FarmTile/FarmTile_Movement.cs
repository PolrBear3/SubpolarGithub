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

    public void Start_Position()
    {
        rectTransform.anchoredPosition = new Vector2(startPosition, rectTransform.anchoredPosition.y); 
    }
    public void Set_Position()
    {
        rectTransform.anchoredPosition = new Vector2(setPosition, rectTransform.anchoredPosition.y);
    }

    public void LeanTween_Start_Position()
    {

    }
    public void LeanTween_Set_Position()
    {

    }
}
