using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodState_Indicator : MonoBehaviour
{
    private SpriteRenderer _sr;

    [Header("Heated State")]
    public SpriteRenderer heatSR;
    public List<Sprite> heatSprites = new();

    [Header("Sliced State")]
    public SpriteRenderer sliceSR;
    public List<Sprite> sliceSprites = new();

    private void Awake()
    {
        if (gameObject.TryGetComponent(out SpriteRenderer sr)) { _sr = sr; }
    }


}
