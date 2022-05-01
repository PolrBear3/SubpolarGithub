using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Box_Holder : MonoBehaviour
{
    private void Awake()
    {
        _boxSystem = new Box_System(boxSize);
    }

    [SerializeField] private int boxSize;
    [SerializeField] protected Box_System _boxSystem;
    public Box_System boxSystem => _boxSystem;

    public static UnityAction<Box_System> boxSlotUpdateRequested;
}
