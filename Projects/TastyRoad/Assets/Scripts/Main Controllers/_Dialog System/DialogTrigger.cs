using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [Header("")]
    [SerializeField] private DialogData _defaultData;
    public DialogData defaultData => _defaultData;

    [SerializeField] private DialogData[] _datas;
    public DialogData[] datas => _datas;


    // Default
    public void Set_DefaultDialog(DialogData setData)
    {
        _defaultData = setData;
    }

    public DialogBox Update_Dialog()
    {
        return Main_Controller.instance.dialogSystem.Add_DialogBox(_defaultData);
    }


    // Array
    public DialogBox Update_Dialog(int dataArrayNum)
    {
        return Main_Controller.instance.dialogSystem.Add_DialogBox(_datas[dataArrayNum]);
    }

    // RunTime String
    public DialogBox Update_Dialog(string info)
    {
        return Main_Controller.instance.dialogSystem.Add_DialogBox(new DialogData(_defaultData.icon, info));
    }

    // RunTime Icon
    public DialogBox Update_Dialog(Sprite icon)
    {
        return Main_Controller.instance.dialogSystem.Add_DialogBox(new DialogData(icon, _defaultData.info));
    }

    // RunTime Data
    public DialogBox Update_Dialog(DialogData newData)
    {
        return Main_Controller.instance.dialogSystem.Add_DialogBox(newData);
    }
}
