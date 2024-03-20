using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Equipment { hand, detector, cuff }

public class Equipment_Controller : MonoBehaviour
{
    private Game_Controller _gameController;

    [SerializeField] private List<Animator> _equipmentAnimators = new();

    public static Equipment currentEquipment;

    [Header("")]
    [SerializeField] private List<Image> _batteryImages = new();

    private int _batteryCount = 6;
    public int batteryCount => _batteryCount;

    [SerializeField] private int _batteryPrice;



    // UnityEngine
    private void Awake()
    {
        _gameController = FindObjectOfType<Game_Controller>();
    }



    // Equip Control
    public void Update_Equipment(int equipNum)
    {
        if (_equipmentAnimators[(int)currentEquipment] != null)
        {
            _equipmentAnimators[(int)currentEquipment].Play("EquipmentUI_idle");
        }

        Equipment targetEquipment = (Equipment)equipNum;

        if (currentEquipment == targetEquipment)
        {
            currentEquipment = Equipment.hand;
            return;
        }

        currentEquipment = targetEquipment;

        _equipmentAnimators[(int)currentEquipment].Play("EquipmentUI_blink");
    }



    // Battery Control
    public void BatteryCount_Update(int updateValue)
    {
        _batteryCount += updateValue;

        int spriteCount = _batteryCount;

        for (int i = 0; i < _batteryImages.Count; i++)
        {
            if (spriteCount > 0)
            {
                _batteryImages[i].color = Color.white;
            }
            else
            {
                _batteryImages[i].color = Color.clear;
            }

            spriteCount -= 2;
        }
    }

    public void Purchase_Battery()
    {
        if (_equipmentAnimators[(int)currentEquipment] != null)
        {
            _equipmentAnimators[(int)currentEquipment].Play("EquipmentUI_idle");
            currentEquipment = Equipment.hand;
        }

        if (_batteryCount >= 5) return;
        if (Data_Controller.score < _batteryPrice) return;

        BatteryCount_Update(2);

        Data_Controller.score -= _batteryPrice;
        _gameController.data.ScoreText_Update();
    }
}
