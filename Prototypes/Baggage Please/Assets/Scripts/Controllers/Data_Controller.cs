using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_Controller : MonoBehaviour
{
    [SerializeField] private GameObject _npcPrefab;
    public GameObject npcPrefab => _npcPrefab;

    [SerializeField] private GameObject _baggagePrefab;
    public GameObject baggagePrefab => _baggagePrefab;

    [SerializeField] private List<BaggageSprite_Data> _bagSpriteDatas = new();
    public List<BaggageSprite_Data> bagSpriteDatas => _bagSpriteDatas;

    public Sprite BaggageSprite(int typeNum, int heatLevel)
    {
        int maxHeatLevel = _bagSpriteDatas[typeNum].bagSprites.Count - 1;

        if (heatLevel >= maxHeatLevel)
        {
            heatLevel = maxHeatLevel;
        }

        return _bagSpriteDatas[typeNum].bagSprites[heatLevel];
    }
}
