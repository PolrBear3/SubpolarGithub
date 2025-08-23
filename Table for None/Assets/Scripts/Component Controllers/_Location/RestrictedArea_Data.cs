using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RestrictedArea_Data 
{
    private SpriteRenderer _areaSR;
    public SpriteRenderer areaSr => _areaSR;

    private GameObject _iRestrictableObject;
    public GameObject iRestrictableObject => _iRestrictableObject;
    
    
    // New
    public RestrictedArea_Data(SpriteRenderer areaSr, GameObject iRestrictableObject)
    {
        _areaSR = areaSr;
        _iRestrictableObject = iRestrictableObject;
    }
}
