using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationData
{
    [SerializeField][ES3Serializable] public Station_ScrObj stationScrObj;
    [SerializeField][ES3Serializable] public int stationID;
    [SerializeField][ES3Serializable] public Vector2 position;

    public StationData(StationData data)
    {
        if (data == null) return;

        stationScrObj = data.stationScrObj;
        stationID = data.stationID;
        position = data.position;
    }

    public StationData(Station_ScrObj stationScrObj)
    {
        this.stationScrObj = stationScrObj;
        stationID = stationScrObj.id;
    }

    public StationData(Station_Controller controller)
    {
        stationScrObj = controller.stationScrObj;
        stationID = controller.stationScrObj.id;
        position = controller.transform.position;
    }
}