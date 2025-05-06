using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MainController : MonoBehaviour
{
    [Space(20)] 
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private int _activateFontSize;
    
    [Space(20)] 
    [SerializeField] private string[] _randStrings;
    [SerializeField] [Range(0, 100)] private int _countDowntime;
    
    
    private Coroutine _coroutine;
    
    
    public void Activate(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _text.fontSize = _activateFontSize;
        _coroutine = StartCoroutine(ActivateCoroutine());
    }

    private IEnumerator ActivateCoroutine()
    {
        int countDowntime = _countDowntime;

        for (int i = 0; i < _countDowntime; i++)
        {
            _text.text = countDowntime.ToString();
            countDowntime--;

            yield return new WaitForSeconds(1f);
        }
        
        string randString = _randStrings[Random.Range(0, _randStrings.Length)];
        _text.text = randString;
        
        _coroutine = null;
        yield break;
    }
}
