using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ New Card")]
public class Card_ScrObj : ScriptableObject
{
    [Space(20)]
    [SerializeField] private string _cardName;
    public string cardName => _cardName;

    [SerializeField] private Sprite _iconSprite;
    public Sprite iconSprite => _iconSprite;
}
