using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour, IInteractable
{
    private SpriteRenderer _sr;

    [Header("")]
    [SerializeField] private Detection_Controller _detection;

    [Header("")]
    [SerializeField][Range(0, 100)] private int _destroyTikCount;
    private int _currentTikCount;


    private ItemSlot_Data _itemData;


    // UnityEngine
    private void Awake()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GlobalTime_Controller.TimeTik_Update += Activate_DestroyTimeTik;
    }

    private void OnDestroy()
    {
        GlobalTime_Controller.TimeTik_Update -= Activate_DestroyTimeTik;
    }


    // IInteractable
    public void Interact()
    {
        Pickup();
    }

    public void Hold_Interact()
    {

    }

    public void UnInteract()
    {

    }


    // Set
    public void Set_ItemData(ItemSlot_Data setData)
    {
        _itemData = new(setData);
    }


    // Functions
    private void Pickup()
    {
        if (_itemData == null) return;

        FoodData_Controller playerIcon = _detection.player.foodIcon;

        if (playerIcon.DataCount_Maxed()) return;

        playerIcon.Set_CurrentData(_itemData.foodData);
        playerIcon.Show_Icon();
        playerIcon.Show_Condition();
        playerIcon.Toggle_SubDataBar(true);

        Destroy(gameObject);
    }

    private void Activate_DestroyTimeTik()
    {
        float alphaStepSize = 100 / _destroyTikCount * 0.01f;

        _currentTikCount++;
        Main_Controller.Change_SpriteAlpha(_sr, _sr.color.a - alphaStepSize);

        if (_currentTikCount < _destroyTikCount) return;
        Destroy(gameObject, 0.1f);
    }
}
