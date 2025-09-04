using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSkin_Controller : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private WorldSkin_Data[] _skinDatas;
    
    
    // Main
    public Sprite[] CurrentWorld_SkinSprites()
    {
        int currentWorldNum = Main_Controller.instance.worldMap.data.currentData.worldNum;
        return _skinDatas[currentWorldNum - 1].skinSprites;
    }
}
