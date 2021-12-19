using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPShop : MonoBehaviour
{
    private string Joys5000 = "com.subpolargames.christmasnightrun.5000joys";
    public void OnPurchaseComplete5000Joys(Product product)
    {
        if (product.definition.id == Joys5000)
        {
            coinText.AddCoins5000();
            Debug.Log("5000Joys purchased");
        }
    }

    private string Joys10000 = "com.subpolargames.christmasnightrun.10000joys";
    public void OnPurchaseComplete10000Joys(Product product)
    {
        if (product.definition.id == Joys10000)
        {
            coinText.AddCoins10000();
            Debug.Log("10000Joys purchased");
        }
    }

    private string Joys30000 = "com.subpolargames.christmasnightrun.30000joys";
    public void OnPurchaseComplete30000Joys(Product product)
    {
        if (product.definition.id == Joys30000)
        {
            coinText.AddCoins30000();
            Debug.Log("30000Joys purchased");
        }
    }

    private string Joys50000 = "com.subpolargames.christmasnightrun.50000joys";
    public void OnPurchaseComplete50000Joys(Product product)
    {
        if (product.definition.id == Joys50000)
        {
            coinText.AddCoins50000();
            Debug.Log("50000Joys purchased");
        }
    }

    private string Joys100000 = "com.subpolargames.christmasnightrun.100000joys";
    public void OnPurchaseComplete100000Joys(Product product)
    {
        if (product.definition.id == Joys100000)
        {
            coinText.AddCoins100000();
            Debug.Log("100000Joys purchased");
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase failed");
    }
}
