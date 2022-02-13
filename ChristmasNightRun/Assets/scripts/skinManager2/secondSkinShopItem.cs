using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class secondSkinShopItem : MonoBehaviour
{
    [SerializeField] private secondSkinManager skinManager2;
    [SerializeField] private int skinIndex2;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text costText;
    private Skin2 skin2;

    void Start()
    {
        skin2 = skinManager2.skins2[skinIndex2];

        GetComponent<Image>().sprite = skin2.sprite2;

        if (skinManager2.IsUnlocked2(skinIndex2))
        {
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            costText.text = skin2.cost2.ToString();
        }
    }

    public void OnSkinPressed2()
    {
        if (skinManager2.IsUnlocked2(skinIndex2))
        {
            skinManager2.SelectSkin2(skinIndex2);
        }
    }

    public void OnBuyButtonPressed2()
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);

        // Unlock the skin
        if (coins >= skin2.cost2 && !skinManager2.IsUnlocked2(skinIndex2))
        {
            PlayerPrefs.SetInt("Coins", coins - skin2.cost2);
            skinManager2.Unlock2(skinIndex2);
            buyButton.gameObject.SetActive(false);
            skinManager2.SelectSkin2(skinIndex2);
        }
        else
        {
            Debug.Log("Not enough coins :(");
        }
    }
}
