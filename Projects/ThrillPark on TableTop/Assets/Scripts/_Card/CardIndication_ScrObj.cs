using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ New Indication")]
public class CardIndication_ScrObj : ScriptableObject
{
    [Space(20)]
    [SerializeField] private string _indicateName;
    public string indicateName => _indicateName;

    [SerializeField] private Sprite _iconSprite;
    public Sprite iconSprite => _iconSprite;
}