using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impediment : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Station_Controller _controller;
    [SerializeField] private WorldSkin_Controller _skinController;
    
    
    // MonoBehaviour
    private void Start()
    {
        // set random skin
        Sprite[] skins = _skinController.CurrentWorld_SkinSprites();
        _controller.spriteRenderer.sprite = skins[Random.Range(0, skins.Length)];
    }
}