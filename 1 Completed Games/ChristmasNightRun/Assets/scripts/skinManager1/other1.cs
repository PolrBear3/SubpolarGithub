using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class other1 : MonoBehaviour
{
    [SerializeField] private skinManager skinManager;
    void Start()
    {
        gameObject.GetComponent<Image>().sprite = skinManager.GetSelectedSkin().sprite;
    }
}
