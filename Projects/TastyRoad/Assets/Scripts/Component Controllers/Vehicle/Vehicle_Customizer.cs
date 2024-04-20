using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle_Customizer : MonoBehaviour
{
    [Header("Body")]
    [SerializeField] private SpriteRenderer _bodySR;
    [SerializeField] private List<VehiclePartData> _bodyData;
    private VehiclePartData _currentBody;

    [Header("Head")]
    [SerializeField] private SpriteRenderer _headSR;
    [SerializeField] private List<VehiclePartData> _headData;
    private VehiclePartData _currentHead;

    [Header("Wheels")]
    [SerializeField] private SpriteRenderer _wheelsSR;
    [SerializeField] private List<VehiclePartData> _wheelsData;
    private VehiclePartData _currentWheels;



    // UnityEngine
    private void Start()
    {
        Customize_All(0, 0, 0);
    }



    //
    private void Customize_All(int bodyID, int headID, int wheelsID)
    {
        _bodySR.sprite = BodyData(bodyID).partScrObj.sprite;
        _headSR.sprite = HeadData(headID).partScrObj.sprite;
        _wheelsSR.sprite = WheelsData(wheelsID).partScrObj.sprite;
    }



    // Search
    private VehiclePartData BodyData(int id)
    {
        for (int i = 0; i < _bodyData.Count; i++)
        {
            if (id != _bodyData[i].partScrObj.id) continue;
            return _bodyData[i];
        }
        return null;
    }

    private VehiclePartData HeadData(int id)
    {
        for (int i = 0; i < _headData.Count; i++)
        {
            if (id != _headData[i].partScrObj.id) continue;
            return _headData[i];
        }
        return null;
    }

    private VehiclePartData WheelsData(int id)
    {
        for (int i = 0; i < _wheelsData.Count; i++)
        {
            if (id != _wheelsData[i].partScrObj.id) continue;
            return _wheelsData[i];
        }
        return null;
    }
}
