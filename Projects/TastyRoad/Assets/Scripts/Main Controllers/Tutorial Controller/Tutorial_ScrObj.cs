using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Tutorial!")]
public class Tutorial_ScrObj : ScriptableObject
{
    public int id;

    public AnimatorOverrideController gifAnimator;

    [TextArea(3, 10)]
    public List<string> explanation;
}
