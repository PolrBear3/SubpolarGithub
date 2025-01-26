using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Controller : MonoBehaviour
{
    private SpriteRenderer _sr;
    public SpriteRenderer sr => _sr;


    private Block _data;
    public Block data => _data;


    // MonoBehaviour
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }


    // Data
    public Block Set_Data(Block setData)
    {
        _data = setData;
        return _data;
    }


    // Visual
    public void Update_CurrentSprite()
    {
        if (_data == null) return;

        _sr.sprite = _data.blockType.blockSprite;
    }
}
