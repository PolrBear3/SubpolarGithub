using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatchet : MonoBehaviour, IEquippable
{
    private Prefab_Controller _controller;

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) { _controller = controller; }
    }

    public void Use()
    {
        Debug.Log("Hatchet Used");
    }
}
