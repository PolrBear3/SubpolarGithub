using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shopController4 : MonoBehaviour
{
    [SerializeField] private Image selectedSkin4;
    [SerializeField] private Text coinsText;
    [SerializeField] private skinManager4 skinManager4;

    void Update()
    {
        coinsText.text = "Coins: " + PlayerPrefs.GetInt("Coins");
        selectedSkin4.sprite = skinManager4.GetSelectedSkin4().sprite4;
    }
}
