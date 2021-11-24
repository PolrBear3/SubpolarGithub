using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class secondShopController : MonoBehaviour
{
    [SerializeField] private Image selectedSkin2;
    [SerializeField] private Text coinsText;
    [SerializeField] private secondSkinManager skinManager2;

    void Update()
    {
        coinsText.text = "Coins: " + PlayerPrefs.GetInt("Coins");
        selectedSkin2.sprite = skinManager2.GetSelectedSkin2().sprite2;
    }
}
