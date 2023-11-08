using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Controller : MonoBehaviour
{
   [HideInInspector] public Customer_Order customerOrder;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Customer_Order customerOrder)) { this.customerOrder = customerOrder; }
    }

    //
}