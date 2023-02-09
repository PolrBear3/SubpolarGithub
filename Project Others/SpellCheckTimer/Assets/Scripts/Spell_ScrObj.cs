using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "New Spell")]
public class Spell_ScrObj : ScriptableObject
{
    public Sprite sprite;
    public Sprite usedSprite;

    public float defaultCoolTime;
    public float bootsCoolTime;
    public float runeCoolTime;
    public float bothCoolTime;
}
