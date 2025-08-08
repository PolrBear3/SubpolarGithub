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

    [Space(20)]
    [SerializeField] private Location_ScrObj[] _locations;
    public Location_ScrObj[] locations => _locations;


    // Gets
    public Location_ScrObj Location_ScrObj(int locationNum)
    {
        int indexNum = locationNum - 1;
        if (indexNum < 0 || indexNum > _locations.Length - 1) return null;
        
        return _locations[indexNum];
    }

    public int LocationCount()
    {
        bool demoBuild = Main_Controller.instance.demoBuild;
        if (demoBuild == false) return _locations.Length;
        
        int locationCount = 0;

        for (int i = 0; i < _locations.Length; i++)
        {
            if (_locations[i].demoBuild == false) continue;
            locationCount++;
        }
        
        return locationCount;
    }
}
