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
    public DialogBox Update_Dialog()
    {
        return _main.dialogSystem.Add_DialogBox(_defaultData);
    }

    // Array
    public DialogBox Update_Dialog(int dataArrayNum)
    {
        return _main.dialogSystem.Add_DialogBox(_datas[dataArrayNum]);
    }

    // RunTime String
    public DialogBox Update_Dialog(string info)
    {
        return _main.dialogSystem.Add_DialogBox(new DialogData(_defaultData.icon, info));
    }

    // RunTime Icon
    public DialogBox Update_Dialog(Sprite icon)
    {
        return _main.dialogSystem.Add_DialogBox(new DialogData(icon, _defaultData.info));
    }

    // RunTime Data
    public DialogBox Update_Dialog(DialogData newData)
    {
        return _main.dialogSystem.Add_DialogBox(newData);
    }
}
