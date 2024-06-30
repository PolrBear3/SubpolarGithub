using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationData
{
    public Station_ScrObj stationScrObj;
    public int stationID;
    public Vector2 position;

    public StationData (Station_Controller controller)
    {
        stationScrObj = controller.stationScrObj;
        stationID = controller.stationScrObj.id;
        position = controller.transform.position;
    }
}