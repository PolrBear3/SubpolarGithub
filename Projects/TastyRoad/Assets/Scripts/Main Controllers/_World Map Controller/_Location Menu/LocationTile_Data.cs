using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationTile_Data
{
    [SerializeField][ES3Serializable] private Location_ScrObj _locationScrObj;
    public Location_ScrObj locationScrObj => _locationScrObj;
}
