using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New ScriptableObject/ New Block")]
public class Block_ScrObj : ScriptableObject
{
    [Header("")]
    [SerializeField] private Sprite _blockSprite;
    public Sprite blockSprite => _blockSprite;

    [SerializeField] private Sprite _setSprite;
    public Sprite setSprite => _setSprite;

    [SerializeField] private Sprite _cellSprite;
    public Sprite cellSprite => _cellSprite;
}
