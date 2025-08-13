using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoldSystem : MonoBehaviour, ISaveLoadable
{
    public static GoldSystem instance;

    [Space(20)]
    [SerializeField] private Image _panel;
    [SerializeField] private Sprite _indicatePanel;

    private Sprite _defaultPanel;

    [Space(20)]
    [SerializeField] private Image _iconImage;

    private Sprite _defaultIcon;
    public Sprite defaultIcon => _defaultIcon;

    [Space(20)]
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private TextMeshProUGUI _shadowText;

    [Space(20)]
    [SerializeField] private Color _red;
    [SerializeField] private Color _green;

    private Color _defaultColor;

    [Space(20)]
    [SerializeField][Range(0, 100)] private float _shakeHeight;
    [SerializeField] private LeanTweenType _shakeTweenType;

    [Space(20)]
    [SerializeField][Range(0, 10)] private float _durationTime;
    [SerializeField][Range(0, 10)] private float _shineSpeed;


    private GoldSystem_Data _data;
    public GoldSystem_Data data => _data;

    public Action<int> OnAmountUpdate;
    
    private Coroutine _coroutine;
    private Coroutine _iconCoroutine;


    // UnityEngine
    private void Awake()
    {
        instance = this;

        _defaultPanel = _panel.sprite;
        _defaultIcon = _iconImage.sprite;
        _defaultColor = _amountText.color;
    }

    private void Start()
    {
        _iconImage.sprite = _defaultIcon;

        if (_data == null) Load_Data();
        Update_CurrentAmount(0);

        // subscriptions
        Vehicle_Controller vehicle = Main_Controller.instance.currentVehicle;

        vehicle.menu.On_MenuToggle += ToggleUpdate_Panel;
        vehicle.locationMenu.On_MenuToggle += ToggleUpdate_Panel;
    }

    private void OnDestroy()
    {
        // subscriptions
        Vehicle_Controller vehicle = Main_Controller.instance.currentVehicle;

        vehicle.menu.On_MenuToggle -= ToggleUpdate_Panel;
        vehicle.locationMenu.On_MenuToggle -= ToggleUpdate_Panel;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("GoldSystem/GoldSystem_Data", _data);
    }

    public void Load_Data()
    {
        GoldSystem_Data data = ES3.Load("GoldSystem/GoldSystem_Data", new GoldSystem_Data(0));
        _data = new(data.goldAmount);
    }


    // Indication
    private void ToggleUpdate_Panel(bool toggle)
    {
        _panel.gameObject.SetActive(!toggle);
    }


    private void Cancel_Coroutine()
    {
        if (_coroutine == null) return;

        StopCoroutine(_coroutine);
        _coroutine = null;

        _panel.sprite = _defaultPanel;

        _iconImage.sprite = _defaultIcon;
        _amountText.text = _data.goldAmount.ToString();

        _amountText.color = _defaultColor;
        _amountText.colorGradient = new VertexGradient(_defaultColor);

        _amountText.text = _data.goldAmount.ToString();
        _shadowText.text = _data.goldAmount.ToString("D7");
        
        _amountText.gameObject.SetActive(_data.goldAmount > 0);
        _shadowText.gameObject.SetActive(true);
    }

    public void Indicate_TriggerData(GoldSystem_TriggerData data)
    {
        Cancel_Coroutine();

        _coroutine = StartCoroutine(Indicate_TriggerData_Coroutine(data));
    }
    private IEnumerator Indicate_TriggerData_Coroutine(GoldSystem_TriggerData data)
    {
        _panel.sprite = _indicatePanel;

        _amountText.gameObject.SetActive(true);
        _shadowText.gameObject.SetActive(false);

        _iconImage.sprite = data.triggerIcon;
        _amountText.text = data.triggerValue.ToString();

        ColorToggle_AmountText(data.triggerValue);

        yield return new WaitForSeconds(_durationTime);

        Cancel_Coroutine();
        yield break;
    }


    private void Shake_Icon()
    {
        RectTransform icon = _iconImage.rectTransform;

        if (LeanTween.isTweening(icon.gameObject)) return;

        float originalY = icon.anchoredPosition.y;
        float duration = _durationTime / 4;

        LeanTween.moveY(icon, icon.anchoredPosition.y + _shakeHeight, duration).setEase(_shakeTweenType)
        .setOnComplete(() =>
        {
            LeanTween.moveY(icon, originalY, duration).setEase(_shakeTweenType);
        });
    }

    private void Shine_Icon()
    {
        if (_iconCoroutine != null) return;

        _iconCoroutine = StartCoroutine(Shine_Icon_Coroutine());
    }
    private IEnumerator Shine_Icon_Coroutine()
    {
        float locationValue = 0.5f;

        while (locationValue < 1)
        {
            locationValue += Time.deltaTime * _shineSpeed;
            _iconImage.material.SetFloat("_ShineLocation", locationValue);

            yield return null;
        }

        _iconCoroutine = null;
        yield break;
    }


    // Calculation
    public bool Update_CurrentAmount(int updateValue)
    {
        int currentAmount = _data.goldAmount;

        if (updateValue == 0)
        {
            _amountText.text = currentAmount.ToString();
            _shadowText.text = currentAmount.ToString("D7");

            _amountText.gameObject.SetActive(currentAmount > 0);

            return true;
        }

        int bonusValue = updateValue >= 0 ? (int)(updateValue * _data.bonusMultiplyAmount) : updateValue;
        int dataBonusValue = updateValue > 0 ? _data.bonusAddAmount : 0;

        int calculateValue = currentAmount + dataBonusValue + bonusValue;

        if (calculateValue < 0)
        {
            Cancel_Coroutine();
            _coroutine = StartCoroutine(ColorToggle_AmountText_DurationCoroutine(-1));

            return false;
        }

        _data.Set_GoldAmount(calculateValue);
        Update_AmountText(currentAmount, calculateValue);

        Shake_Icon();
        Shine_Icon();

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

        Cancel_Coroutine();
        yield break;
    }


    private void ColorToggle_AmountText(int triggerValue)
    {
        if (triggerValue > 0)
        {
            _amountText.color = _green;
            _amountText.colorGradient = new VertexGradient(_green);

            return;
        }

        _amountText.color = _red;
        _amountText.colorGradient = new VertexGradient(_red);
    }

    private IEnumerator ColorToggle_AmountText_DurationCoroutine(int triggerValue)
    {
        _amountText.gameObject.SetActive(true);

        ColorToggle_AmountText(triggerValue);
        yield return new WaitForSeconds(_durationTime);

        Cancel_Coroutine();
        yield break;
    }
}
