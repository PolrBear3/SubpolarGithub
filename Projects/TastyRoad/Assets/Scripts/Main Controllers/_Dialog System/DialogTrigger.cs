using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    private Main_Controller _main;

    [Header("")]
    [SerializeField] private DialogData _defaultData;
    public DialogData defaultData => _defaultData;

    [SerializeField] private DialogData[] _datas;
    public DialogData[] datas => _datas;


    // MonoBehaviour
    private void Awake()
    {
        _main = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }


    // gameObject.GetComponent<DialogTrigger>().Update_Dialog();


    public void Set_DefaultDialog(DialogData setData)
    {
        _defaultData = setData;
    }


    // Default
    public void Update_Dialog()
    {
        _main.dialogSystem.Add_DialogBox(_defaultData);
    }

    // Array
    public void Update_Dialog(int dataArrayNum)
    {
        _main.dialogSystem.Add_DialogBox(_datas[dataArrayNum]);
    }

    // RunTime String
    public void Update_Dialog(string info)
    {
        _main.dialogSystem.Add_DialogBox(new DialogData(_defaultData.icon, info));
    }

    // RunTime Icon
    public void Update_Dialog(Sprite icon)
    {
        _main.dialogSystem.Add_DialogBox(new DialogData(icon, _defaultData.info));
    }

    // RunTime Data
    public void Update_Dialog(DialogData newData)
    {
        _main.dialogSystem.Add_DialogBox(newData);
    }
}
