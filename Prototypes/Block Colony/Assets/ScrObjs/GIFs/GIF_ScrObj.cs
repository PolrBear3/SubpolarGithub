using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New GIF")]
public class GIF_ScrObj : ScriptableObject
{
    public AnimatorOverrideController animOverrider;

    [TextArea(3, 10)]
    public string description;
}
