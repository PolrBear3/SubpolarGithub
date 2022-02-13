using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skinShopItem4 : MonoBehaviour
{
    [SerializeField] private skinManager4 skinManager4;
    [SerializeField] private int skinIndex4;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text costText;
    private Skin4 skin4;
    
    void Start()
    {
        skin4 = skinManager4.skins4[skinIndex4];

        GetComponent<Image>().sprite = skin4.sprite4;

        if (skinManager4.IsUnlocked4(skinIndex4))
        {
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            costText.text = skin4.cost4.ToString();
        }
    }

    public void OnSkinPressed4()
    {
        if (skinManager4.IsUnlocked4(skinIndex4))
        {
            skinManager4.SelectSkin4(skinIndex4);
        }
    }

    public void OnBuyButtonPressed4()
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);

        // Unlock the skin
        if (coins >= skin4.cost4 && !skinManager4.IsUnlocked4(skinIndex4))
        {
            PlayerPrefs.SetInt("Coins", coins - skin4.cost4);
            skinManager4.Unlock4(skinIndex4);
            buyButton.gameObject.SetActive(false);
            skinManager4.SelectSkin4(skinIndex4);
        }
        else
        {
            Debug.Log("Not enough coins :(");
        }
    }
}
