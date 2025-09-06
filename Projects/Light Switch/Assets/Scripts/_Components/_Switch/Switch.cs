using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    
    [Space(20)]
    [SerializeField] private Clickable_EventSystem _eventSystem;


    private Switch_Data _data;
    public Switch_Data data => _data;
    
    private Coroutine _coroutine;
    
    
    // MonoBehaviour
    private void Start()
    {
        _eventSystem.OnClick += Update_TotalToggleCount;
        _eventSystem.OnClick += Toggle;
    }

    private void OnDestroy()
    {
        _eventSystem.OnClick -= Update_TotalToggleCount;
        _eventSystem.OnClick -= Toggle;
    }

    
    // Get
    private float Current_ToggleCoolTime()
    {
        return _data.switchSrcObj.toggleCooltime;
    }
    
    
    // Data
    public void Set_Data(Switch_Data data)
    {
        _data = data;
    }

    public void Load_DataIndication()
    {
        _sr.color = _data.switchSrcObj.switchColor;
    }
    

    // Main
    private void Toggle()
    {
        if (_coroutine != null) return;
        _coroutine = StartCoroutine(Toggle_Coroutine());
    }
    private IEnumerator Toggle_Coroutine()
    {
        _sr.color = Color.red;
        
        float cooltime = Current_ToggleCoolTime();
        yield return new WaitForSeconds(cooltime);
        
        _sr.color = _data.switchSrcObj.switchColor;
        _coroutine = null;
    }

    private void Update_TotalToggleCount()
    {
        if (_coroutine != null) return;
        
        Inventory inventory = Main_Controller.instance.inventory;
        int setValue = inventory.data.totalToggleCount + _data.switchSrcObj.toggleCount;
        
        inventory.data.Set_TotalToggleCount(setValue);
        inventory.Update_TotalToggleText();
    }
}
