using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    [SerializeField] private Sprite _worldIcon;
    public Sprite worldIcon => _worldIcon;

    [SerializeField] private Sprite _panelSprite;
    public Sprite panelSprite => _panelSprite;

    [SerializeField] private AnimatorOverrideController _tileAnimOverrider;
    public AnimatorOverrideController tileAnimOverrider => _tileAnimOverrider;

    [Header("")]
    [SerializeField] private Location_ScrObj[] _locations;
    public Location_ScrObj[] locations => _locations;
}
