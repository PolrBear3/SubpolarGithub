using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class coinText : MonoBehaviour
{
    [SerializeField] private Text coinsText;
    private static int coins;

    void Start()
    {
        coins = PlayerPrefs.GetInt("Coins");
    }

    void Update()
    {
        coinsText.text = "" + coins;
    }

    public static void AddCoins()
    {
        coins += 1;
    }

    public static void AddCoinsforAd()
    {
        coins += 100;
    }
    
    public static void AddCoins5000()
    {
        coins += 5000;
    }
    public static void AddCoins10000()
    {
        coins += 10000;
    }
    public static void AddCoins30000()
    {
        coins += 30000;
    }
    public static void AddCoins50000()
    {
        coins += 50000;
    }
    public static void AddCoins100000()
    {
        coins += 100000;
    }

    public static void coinSave()
    {
        PlayerPrefs.SetInt("Coins", coins);
    }
}
