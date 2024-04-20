using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle_Customizer : MonoBehaviour, ISaveLoadable
{
    [Header("Body")]
    [SerializeField] private SpriteRenderer _bodySR;
    public SpriteRenderer bodySR => _bodySR;

    [SerializeField] private List<VehiclePartData> _bodyDatas;

    private VehiclePartData _currentBody;

    [Header("Head")]
    [SerializeField] private SpriteRenderer _headSR;
    public SpriteRenderer headSR => _headSR;

    [SerializeField] private List<VehiclePartData> _headDatas;

    private VehiclePartData _currentHead;

    [Header("Wheels")]
    [SerializeField] private SpriteRenderer _wheelsSR;
    public SpriteRenderer wheelsSR => _wheelsSR;

    [SerializeField] private List<VehiclePartData> _wheelsDatas;

    private VehiclePartData _currentWheels;



    // UnityEngine
    private void Start()
    {
        Update_CurrentBody(0);

        Update_CurrentHead(0);

        Update_CurrentWheels(0);
    }



    // ISaveLoadable
    public void Save_Data()
    {
        
    }

    public void Load_Data()
    {
        
    }



    // Customize
    public void Update_CurrentBody(int updateNum)
    {
        _currentBody = BodyData(BodyData_ArrayNum(_currentBody) + updateNum);
        _bodySR.sprite = _currentBody.partScrObj.sprite;
    }

    public void Update_CurrentHead(int updateNum)
    {
        _currentHead = HeadData(HeadData_ArrayNum(_currentHead) + updateNum);
        _headSR.sprite = _currentHead.partScrObj.sprite;
    }

    public void Update_CurrentWheels(int updateNum)
    {
        _currentWheels = WheelsData(WheelsData_ArrayNum(_currentWheels) + updateNum);
        _wheelsSR.sprite = _currentWheels.partScrObj.sprite;
    }



    // Body Search
    private int BodyData_ArrayNum(VehiclePartData data)
    {
        for (int i = 0; i < _bodyDatas.Count; i++)
        {
            if (data != _bodyDatas[i]) continue;
            return i;
        }
        return 0;
    }

    private VehiclePartData BodyData(int arrayNum)
    {
        if (arrayNum > _bodyDatas.Count - 1) arrayNum = 0;
        else if (arrayNum < -1) arrayNum = _bodyDatas.Count - 1;

        return _bodyDatas[arrayNum];
    }

    // Head Search
    private int HeadData_ArrayNum(VehiclePartData data)
    {
        for (int i = 0; i < _headDatas.Count; i++)
        {
            if (data != _headDatas[i]) continue;
            return i;
        }
        return 0;
    }

    private VehiclePartData HeadData(int arrayNum)
    {
        if (arrayNum > _headDatas.Count - 1) arrayNum = 0;
        else if (arrayNum < -1) arrayNum = _headDatas.Count - 1;

        return _headDatas[arrayNum];
    }

    // Wheels Search
    private int WheelsData_ArrayNum(VehiclePartData data)
    {
        for (int i = 0; i < _wheelsDatas.Count; i++)
        {
            if (data != _wheelsDatas[i]) continue;
            return i;
        }
        return 0;
    }

    private VehiclePartData WheelsData(int arrayNum)
    {
        if (arrayNum > _wheelsDatas.Count - 1) arrayNum = 0;
        else if (arrayNum < -1) arrayNum = _wheelsDatas.Count - 1;

        return _wheelsDatas[arrayNum];
    }
}
