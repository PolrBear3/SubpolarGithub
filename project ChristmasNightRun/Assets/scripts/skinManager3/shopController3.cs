using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shopController3 : MonoBehaviour
{
    [SerializeField] private Image selectedSkin3;
    [SerializeField] private Text coinsText;
    [SerializeField] private skinManager3 skinManager3;

    void Update()
    {
        coinsText.text = "Coins: " + PlayerPrefs.GetInt("Coins");
        selectedSkin3.sprite = skinManager3.GetSelectedSkin3().sprite3;
    }
}
