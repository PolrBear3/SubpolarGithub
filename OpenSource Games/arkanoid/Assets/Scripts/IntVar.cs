using System;
using UnityEngine;

[CreateAssetMenu]
public class IntVar : ScriptableObject, ISerializationCallbackReceiver
{
    public int SavedValue;

    [NonSerialized]
    public int Value;

    public void OnAfterDeserialize()
    {
        Value = SavedValue;
    }

    public void OnBeforeSerialize()
    {
    }

    public void Reset()
    {
        Value = SavedValue;
    }
}