using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StationShopNPC_Data : MonoBehaviour
{
    [ES3Serializable] private List<StationData> _archiveDatas = new();
    public List<StationData> archiveDatas => _archiveDatas;

    [ES3Serializable] private Station_ScrObj _buildStation;
    public Station_ScrObj buildStation => _buildStation;
}
