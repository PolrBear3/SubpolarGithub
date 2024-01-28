using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VechiclePanel_ItemBox : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;

    [Header("")]
    [SerializeField] private Image _selectIcon;
    [SerializeField] private Image _bookmarkIcon;

    [HideInInspector] public int boxNum;
    [HideInInspector] public Vector2 gridNum;

    [HideInInspector] public bool hasItem;
    [HideInInspector] public bool bookMarked;

    [HideInInspector] public int currentAmount;

    private Food_ScrObj _currentFood;
    public Food_ScrObj currentFood => _currentFood;

    [Header("")]
    [SerializeField] private GameObject _stateIndicator;
    [SerializeField] private List<StateBox_Sprite> stateBoxSprites = new();
    [SerializeField] private List<Image> stateBoxImages = new();

    // UnityEngine
    private void Awake()
    {
        _iconImage.color = Color.clear;
        _amountText.color = Color.clear;
        _selectIcon.color = Color.clear;
        _bookmarkIcon.color = Color.clear;

        _stateIndicator.SetActive(false);
    }

    // Icon control
    public void BoxSelect_Toggle(bool isSelected)
    {
        if (isSelected == true)
        {
            _selectIcon.color = Color.white;

            return;
        }

        _selectIcon.color = Color.clear;
    }

    public void Toggle_BookMark()
    {
        if (hasItem == false) return;

        if (bookMarked == false)
        {
            bookMarked = true;
            _bookmarkIcon.color = Color.white;

            return;
        }

        bookMarked = false;
        _bookmarkIcon.color = Color.clear;
    }

    // sprite update included
    public void Assign_Item(Food_ScrObj food)
    {
        if (food != null)
        {
            hasItem = true;
            _currentFood = food;

            _iconImage.sprite = food.sprite;

            _iconImage.color = Color.white;
            _iconImage.transform.localPosition = food.centerPosition;

            return;
        }

        hasItem = false;
        _currentFood = null;

        _iconImage.sprite = null;
        _iconImage.color = Color.clear;

        _iconImage.transform.localPosition = Vector2.zero;

        Assign_Amount(0);
    }

    // text update included
    public void Assign_Amount(int assignAmount)
    {
        currentAmount = assignAmount;

        if (hasItem == false || currentAmount <= 0)
        {
            _currentFood = null;
            _amountText.color = Color.clear;

            return;
        }

        _amountText.text = currentAmount.ToString();
        _amountText.color = Color.black;
    }
    public void Update_Amount(int updateAmount)
    {
        currentAmount += updateAmount;

        if (hasItem == false || currentAmount <= 0)
        {
            _currentFood = null;
            _amountText.color = Color.clear;

            return;
        }

        _amountText.text = currentAmount.ToString();
        _amountText.color = Color.black;
    }

    // StateBox Image Control
    private Sprite StateBox_Sprite(FoodState_Type type, int level)
    {
        for (int i = 0; i < stateBoxSprites.Count; i++)
        {
            if (stateBoxSprites[i].type == type)
            {
                return stateBoxSprites[i].boxSprites[level - 1];
            }
        }
        return null;
    }

    public void Assign_State(List<FoodState_Data> stateData)
    {
        if (stateData.Count <= 0) return;

        _stateIndicator.SetActive(true);

        for (int i = 0; i < stateBoxImages.Count; i++)
        {
            stateBoxImages[i].sprite = null;
            stateBoxImages[i].color = Color.clear;
        }

        for (int i = 0; i < stateData.Count; i++)
        {
            for (int j = 0; j < stateBoxImages.Count; j++)
            {
                if (stateBoxImages[j].sprite != null) continue;

                stateBoxImages[j].sprite = StateBox_Sprite(stateData[i].stateType, stateData[i].stateLevel);
                stateBoxImages[i].color = Color.white;
            }
        }
    }
}