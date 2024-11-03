# Editor Scripting
```C#
using UnityEditor;
```

### Target Inspector Scripts
```C#
public class Item_ScrObj : ScriptableObject
{
    public string itemName;
}
```
```C#
public class ItemController : MonoBehaviour
{
    private List<Item_ScrObj> _currentItems = new();

    // for editor prop
    public Item_ScrObj itemToAdd

    public void Add_Item(Item_ScrObj addItem)
    {
        _currentItems.Add(addItem);
    }
}
```

### Main Template
```C#
#if UNITY_EDITOR
[CustomEditor(typeof(ItemController))]
public class ItemController : Editor
{
    private SerializedProperty itemToAddProp;

    private void OnEnable()
    {
        itemToAddProp = serializedObject.FindProperty("itemToAdd");
    }

    public override void OnInspectorGUI()
    {
        ItemController itemController = (ItemController)target;
        
        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.Space(60);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(itemToAddProp, GUIContent.none);
        Item_ScrObj itemToAdd = (Item_ScrObj)itemToAddProp.objectReferenceValue;

        if (GUILayout.Button("Add Item"))
        {
            itemController.Add_Item(itemToAdd);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
```