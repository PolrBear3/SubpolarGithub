using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPShop : MonoBehaviour
{
    private string Joys = "com.SubpolarGames.ChristmasNightRun.10000Joys";
    
    public void OnPurchaseComplete10000Joys(Product product)
    {
        if (product.definition.id == Joys)
        {
            coinText.AddCoins10000();
            Debug.Log("10000Joys purchased");
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase failed");
    }
}
