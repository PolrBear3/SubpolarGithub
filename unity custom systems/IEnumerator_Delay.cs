using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IEnumerator_Delay : MonoBehaviour
{
    // delay between loops of functions in systems
    // System script has Function();    
    
    private System[] systems;
    private float delayTime;

    private void Start()
    {
        StartCoroutine(Activate_Functions_withDelay(delayTime))
    }

    private IEnumerator Activate_Functions_withDelay(float delayTime)
    {
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Function();
            yield return new WaitForSeconds(delayTime);
        }
    }
}