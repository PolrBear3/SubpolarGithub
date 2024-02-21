using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationData
{
    public int stationID;
    public Vector2 position;

    public StationData (Station_Controller controller)
    {
        stationID = controller.stationScrObj.id;
        position = controller.transform.position;
    }
}