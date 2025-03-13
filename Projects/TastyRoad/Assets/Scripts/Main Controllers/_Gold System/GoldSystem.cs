using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoldSystem : MonoBehaviour
{
    public static GoldSystem instance;


    [Header("")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Sprite _defaultIcon;

    [Header("")]
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private TextMeshProUGUI _shadowText;

    [Header("")]
    [SerializeField][Range(0, 10)] private float _durationTime;


    private int _currentAmount;
    public int currentAmont => _currentAmount;


    private Coroutine _coroutine;


    // UnityEngine
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _iconImage.sprite = _defaultIcon;

        // _currentAmount = ES3.load //
        Update_CurrentAmount(0);
    }


    // Indication
    private void Cancel_Coroutine()
    {
        if (_coroutine == null) return;

        StopCoroutine(_coroutine);
        _coroutine = null;

        _iconImage.sprite = _defaultIcon;
        _amountText.text = _currentAmount.ToString();
    }

    public void Indicate_TriggerData(GoldSystem_TriggerData data)
    {
        Cancel_Coroutine();

        _coroutine = StartCoroutine(Indicate_TriggerData_Coroutine(data));
    }
    private IEnumerator Indicate_TriggerData_Coroutine(GoldSystem_TriggerData data)
    {
        _iconImage.sprite = data.triggerIcon;
        _amountText.text = data.triggerValue.ToString();

        // text color //

        yield return new WaitForSeconds(_durationTime);

        _iconImage.sprite = _defaultIcon;
        _amountText.text = _currentAmount.ToString();

        // text color //

        _coroutine = null;
        yield break;
    }


    // Calculation
    public bool Update_CurrentAmount(int updateValue)
    {
        if (updateValue == 0)
        {
            _amountText.text = _currentAmount.ToString();

            _amountText.gameObject.SetActive(_currentAmount > 0);
            return true;
        }

        int calculateValue = _currentAmount + updateValue;

        if (calculateValue < 0)
        {
            // restricted update indication //
            return false;
        }

        // panel shake animation //

        Update_AmountText(_currentAmount, calculateValue);
        _currentAmount = calculateValue;

        return true;
    }


    // Text
    private void Update_AmountText(int previousValue, int updateValue)
    {
        Cancel_Coroutine();

        _coroutine = StartCoroutine(Update_AmountText_Coroutine(previousValue, updateValue));
    }
    private IEnumerator Update_AmountText_Coroutine(int previousValue, int updateValue)
    {
        float elapsedTime = 0f;

        _amountText.gameObject.SetActive(true);

        while (elapsedTime < _durationTime)
        {
            elapsedTime += Time.deltaTime;
            int textAmount = Mathf.RoundToInt(Mathf.Lerp(previousValue, updateValue, elapsedTime / _durationTime));

            _amountText.text = textAmount.ToString();
            _shadowText.text = textAmount.ToString("D7");

            yield return null;
        }

        _amountText.text = _currentAmount.ToString();
        _shadowText.text = _currentAmount.ToString("D7");

        _amountText.gameObject.SetActive(_currentAmount > 0);

        _coroutine = null;
        yield break;
    }
}
