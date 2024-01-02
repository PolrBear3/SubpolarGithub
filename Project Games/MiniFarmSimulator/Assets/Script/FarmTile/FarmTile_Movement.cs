using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmTile_Movement : MonoBehaviour
{
    private FarmTile farmTile;
    
    private RectTransform rectTransform;
    [SerializeField] private LeanTweenType tweenType;

    [SerializeField] private RectTransform previousTileSprite;
    [SerializeField] private RectTransform harvestBorder;
    [SerializeField] private RectTransform deathIcon;

    [SerializeField] private float startPosition;
    [SerializeField] private float setPosition;

    [SerializeField] private float speed;
    [SerializeField] private float fadeDelayTime;

    [SerializeField] private float startDelaytime;
    [SerializeField] private float setDelayTime;

    private void Awake()
    {
        farmTile = GetComponent<FarmTile>();
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

        var previousTileImage = previousTileSprite.GetComponent<Image>();
        previousTileImage.sprite = farmTile.image.sprite;

        LeanTween.alpha(previousTileSprite, 1f, delayTime);
        LeanTween.alpha(harvestBorder, 0f, delayTime);

        if (farmTile.deathData.died) return;
        LeanTween.alpha(deathIcon, 0f, delayTime);
    }
    public void LeanTween_Set_Position(float delayTime)
    {
        LeanTween.move(rectTransform, new Vector2(setPosition, rectTransform.anchoredPosition.y), speed).setEase(tweenType).setDelay(setDelayTime + delayTime);

        LeanTween.alpha(previousTileSprite, 0f, delayTime);
        LeanTween.alpha(harvestBorder, 1f, delayTime);

        if (farmTile.deathData.died) return;
        LeanTween.alpha(deathIcon, 1f, delayTime);
    }
}
