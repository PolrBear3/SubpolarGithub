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
        GroceryNPC targetScript = (GroceryNPC)target;

        GUILayout.Space(60);

        if (GUILayout.Button("Activate"))
        {
            targetScript.Collect_FoodBundles();
        }
    }
}
#endif
