using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideData
{
    [ES3Serializable] private Guide_ScrObj _guideScrObj;
    public Guide_ScrObj guideScrObj => _guideScrObj;


    public GuideData(Guide_ScrObj guideScrObj)
    {
        _guideScrObj = guideScrObj;
    }
}
