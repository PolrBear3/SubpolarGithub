using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(GroceryNPC))]
public class EditorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GroceryNPC npc = (GroceryNPC)target;

        GUILayout.Space(60);

        if (GUILayout.Button("Activate"))
        {
            npc.Set_Discount();
        }
    }
}
#endif
