using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class SoundData_Controller : MonoBehaviour
{
    [SerializeField] private SoundData[] _soundDatas;
    public SoundData[] soundDatas => _soundDatas;
    
    [Header("")]
    [SerializeField][Range(0, 100)] private float _fadeDuration;
    public float fadeDuration => _fadeDuration;
    

    // MonoBehaviour
    private void OnDestroy()
    {
        Audio_Controller.instance.Clear_EventInstances(_soundDatas);
    }
    
    
    // Parameter Automation Control
    public void FadeOut(int dataIndex)
    {
        Audio_Controller audio = Audio_Controller.instance;

        SoundData data = audio.EventInstance_Data(_soundDatas[dataIndex]);
        if (data == null) return;
 
        StartCoroutine(FadeOut_Coroutine(audio.EventInstance(data)));
    }
    private IEnumerator FadeOut_Coroutine(EventInstance instance)
    {
        float startValue = 1f;
        instance.getParameterByName("Value_intensity", out startValue);
        
        float elapsed = 0f;
        
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Lerp(startValue, 0f, elapsed / _fadeDuration);
            
            instance.setParameterByName("Value_intensity", value);
            yield return null;
        }
        
        yield break;
    }
}