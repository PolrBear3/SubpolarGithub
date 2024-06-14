using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogData[] _datas;


    // Set Update
    public void Update_Dialog(Main_Controller main, int dataArrayNum)
    {
        main.dialogSystem.Add_DialogBox(_datas[dataArrayNum]);
    }

    // RunTime Update
    public void Update_Dialog(Main_Controller main, DialogData newData)
    {
        main.dialogSystem.Add_DialogBox(newData);
    }
}
