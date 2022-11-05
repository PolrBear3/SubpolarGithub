using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MainGame_Controller))]
public class Money_Button : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MainGame_Controller controller = (MainGame_Controller)target;
        if (GUILayout.Button("Add Money"))
        {
            controller.Add_Money(controller.addMoneyAmount);
        }
    }
}
