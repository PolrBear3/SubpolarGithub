using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateBox_Sprite
{
    public bool isTransparent;

    public FoodCondition_Type type;
    public List<Sprite> boxSprites = new();
}

public class StateBox_Controller : MonoBehaviour
{
    
}