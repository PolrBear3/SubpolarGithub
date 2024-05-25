using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle_Customizer : MonoBehaviour, ISaveLoadable
{
    [Header("Body")]
    [SerializeField] private SpriteRenderer _bodySR;
    [SerializeField] private List<VehiclePartData> _bodyDatas;

    private VehiclePartData _currentBody;

    [Header("Head")]
    [SerializeField] private SpriteRenderer _headSR;
    [SerializeField] private List<VehiclePartData> _headDatas;

    private VehiclePartData _currentHead;

    [Header("Wheels")]
    [SerializeField] private SpriteRenderer _wheelsSR;
    [SerializeField] private List<VehiclePartData> _wheelsDatas;

    private VehiclePartData _currentWheels;


    // UnityEngine
    private void Start()
    {
        Main_Controller.TestButton1Event += Set_Body;
        Main_Controller.TestButton2Event += Set_Head;
        Main_Controller.TestButton3Event += Set_Wheels;
    }

    private void OnDestroy()
    {
        Main_Controller.TestButton1Event -= Set_Body;
        Main_Controller.TestButton2Event -= Set_Head;
        Main_Controller.TestButton3Event -= Set_Wheels;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("Vehicle_Customizer/_currentBody", _currentBody);
        ES3.Save("Vehicle_Customizer/_currentHead", _currentHead);
        ES3.Save("Vehicle_Customizer/_currentWheels", _currentWheels);
    }

    public void Load_Data()
    {
        Set_Body(BodyData_ArrayNum(ES3.Load("Vehicle_Customizer/_currentBody", _currentBody)));
        Set_Head(HeadData_ArrayNum(ES3.Load("Vehicle_Customizer/_currentHead", _currentHead)));
        Set_Wheels(WheelsData_ArrayNum(ES3.Load("Vehicle_Customizer/_currentWheels", _currentWheels)));
    }


    // Test Buttons
    public void Set_Body()
    {
        int arrayNum = BodyData_ArrayNum(_currentBody) + 1;
        Set_Body(arrayNum);
    }

    public void Set_Head()
    {
        int arrayNum = HeadData_ArrayNum(_currentHead) + 1;
        Set_Head(arrayNum);
    }

    public void Set_Wheels()
    {
        int arrayNum = WheelsData_ArrayNum(_currentWheels) + 1;
        Set_Wheels(arrayNum);
    }


    // Customize
    public void Set_Body(int arrayNum)
    {
        int indexNum = arrayNum % _bodyDatas.Count;

        _currentBody = _bodyDatas[indexNum];
        _bodySR.sprite = _currentBody.partScrObj.sprite;
    }

    public void Set_Head(int arrayNum)
    {
        int indexNum = arrayNum % _headDatas.Count;

        _currentHead = _headDatas[indexNum];
        _headSR.sprite = _currentHead.partScrObj.sprite;
    }

    public void Set_Wheels(int arrayNum)
    {
        int indexNum = arrayNum % _wheelsDatas.Count;

        _currentWheels = _wheelsDatas[indexNum];
        _wheelsSR.sprite = _currentWheels.partScrObj.sprite;
    }


    // Search
    private int BodyData_ArrayNum(VehiclePartData data)
    {
        if (data == null) return 0;

        for (int i = 0; i < _bodyDatas.Count; i++)
        {
            if (data.partScrObj != _bodyDatas[i].partScrObj) continue;
            return i;
        }

        return 0;
    }

    private int HeadData_ArrayNum(VehiclePartData data)
    {
        if (data == null) return 0;

        for (int i = 0; i < _headDatas.Count; i++)
        {
            if (data.partScrObj != _headDatas[i].partScrObj) continue;
            return i;
        }
        return 0;
    }

    private int WheelsData_ArrayNum(VehiclePartData data)
    {
        if (data == null) return 0;

        for (int i = 0; i < _wheelsDatas.Count; i++)
        {
            if (data.partScrObj != _wheelsDatas[i].partScrObj) continue;
            return i;
        }
        return 0;
    }
}
