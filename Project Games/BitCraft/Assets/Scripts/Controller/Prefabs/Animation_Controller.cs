using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Controller : MonoBehaviour
{
    private Prefab_Controller _controller;

    [SerializeField] private List<Sprite> _updateSprites = new List<Sprite>();

    //
    private void Awake()
    {
        if (gameObject.TryGetComponent(out Prefab_Controller controller)) _controller = controller;
    }

    //
    public void Update_Sprite(int spriteNum)
    {
        _controller.sr.sprite = _updateSprites[spriteNum];
    }
}
