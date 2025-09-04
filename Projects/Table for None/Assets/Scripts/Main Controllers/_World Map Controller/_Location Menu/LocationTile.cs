using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationTile : MonoBehaviour
{
    private Image _image;

    private Animator _anim;
    public Animator anim => _anim;


    [Header("")]
    [SerializeField] private Sprite _lockedTile;


    [Header("")]
    [SerializeField] private RectTransform _cursorPoint;
    public RectTransform cursorPoint => _cursorPoint;

    private bool _locked;
    public bool locked => _locked;

    private WorldMap_Data _data;
    public WorldMap_Data data => _data;


    // UnityEngine
    private void Awake()
    {
        _image = gameObject.GetComponent<Image>();
        _anim = gameObject.GetComponent<Animator>();
    }


    // Sets
    public void Set_WorldMapData(WorldMap_Data data)
    {
        _data = new(data);
    }

    public Animator Set_AnimatorOverrider(AnimatorOverrideController overrider)
    {
        _anim.runtimeAnimatorController = overrider;
        return _anim;
    }


    // Controls
    public void Toggle_Lock(bool toggle)
    {
        _locked = toggle;
        _anim.enabled = !toggle;

        if (toggle == false) return;

        _image.sprite = _lockedTile;
    }
}
