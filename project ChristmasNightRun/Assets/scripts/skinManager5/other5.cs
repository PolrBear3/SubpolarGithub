using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class other5 : MonoBehaviour
{
    [SerializeField] private skinManager5 skinManager5;
    void Start()
    {
        gameObject.GetComponent<Image>().sprite = skinManager5.GetSelectedSkin5().sprite5;
    }
}
