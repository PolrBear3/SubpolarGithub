using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLight_Controller : MonoBehaviour
{
    [SerializeField] private Light2D _globalLight;
    public Light2D globalLight => _globalLight;

    
    private Color _defaultColor;
    public Color defaultColor => _defaultColor;

    private bool _updateLocked;
    private Coroutine _coroutine;
    
    
    // Data
    public void Set_DefaultColor(Color color)
    {
        _defaultColor = color;
    }

    
    // Current Color
    public void Toggle_CurrentColorLock(bool toggle)
    {
        _updateLocked = toggle;
    }
    
    public void Update_CurrentColor(Color updateColor, float transitionDuration)
    {
        if (_updateLocked) return;
        if (updateColor == _globalLight.color) return;
        
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _coroutine = StartCoroutine(Update_CurrentColor_Coroutine(updateColor, transitionDuration));
    }
    private IEnumerator Update_CurrentColor_Coroutine(Color updateColor, float transitionDuration)
    {
        Color startColor = _globalLight.color;
        float updateTime = 0f;

        while (updateTime < transitionDuration)
        {
            updateTime += Time.deltaTime;
            
            float time = updateTime / transitionDuration;
            _globalLight.color = Color.Lerp(startColor, updateColor, time);

            yield return null;
        }

        _globalLight.color = updateColor;

        _coroutine = null;
        yield break;
    }
}
