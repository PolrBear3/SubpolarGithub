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
    [SerializeField] private float fadeDelayTime;

    [SerializeField] private float startDelaytime;
    [SerializeField] private float setDelayTime;

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

    public void LeanTween_Start_Position(float delayTime)
    {
        LeanTween.move(rectTransform, new Vector2(startPosition, rectTransform.anchoredPosition.y), speed).setEase(tweenType).setDelay(startDelaytime + delayTime);
    }
    public void LeanTween_Set_Position(float delayTime)
    {
        LeanTween.move(rectTransform, new Vector2(setPosition, rectTransform.anchoredPosition.y), speed).setEase(tweenType).setDelay(setDelayTime + delayTime);
    }
}
