using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public interface IAbility
{
    void Activate();
}

public class AbilityManager : MonoBehaviour, ISaveLoadable
{
    [Space(20)]
    [SerializeField] private Player_Controller _player;
    public Player_Controller player => _player;

    [SerializeField] private AmountBar _abilityBar;
    
    [Space(20)] 
    [SerializeField] private Sprite[] _abilityPointIcons;
    
    [Space(20)]
    [SerializeField] private Ability_Behaviour[] _allAbilities;
    public Ability_Behaviour[] allAbilities => _allAbilities;
    
    [SerializeField][Range(0, 100)] private int _maxAbilityPoint;
    public int maxAbilityPoint => _maxAbilityPoint;
    

    private AbilityManager_Data _data;
    public AbilityManager_Data data => _data;
    
    private Coroutine _coroutine;

    
    public static Action<int> IncreasePoint;
    public static Action OnMaxPoint;


    // Editor
    [HideInInspector] public Ability_ScrObj editorAbility;


    // MonoBehaviour
    private void Start()
    {
        Load_AbilityDatas();
        Toggle_AbilityBar();

        // subscriptions
        IncreasePoint += Increase_AbilityPoint;

        _player.OnHideToggle += Toggle_AbilityBar;
        _player.foodIcon.OnCurrentDataSet += Toggle_AbilityBar;
    }

    private void OnDestroy()
    {
        // subscriptions
        IncreasePoint -= Increase_AbilityPoint;
        
        _player.OnHideToggle -= Toggle_AbilityBar;
        _player.foodIcon.OnCurrentDataSet -= Toggle_AbilityBar;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("AbilityManager/AbilityManager_Data", _data);
    }

    public void Load_Data()
    {
        _data = ES3.Load("AbilityManager/AbilityManager_Data", new AbilityManager_Data(0));
    }


    // Data
    private void Load_AbilityDatas()
    {
        for (int i = 0; i < _allAbilities.Length; i++)
        {
            Ability abilityData = _data.AbilityData(_allAbilities[i].abilityScrObj);
            if (abilityData == null)
            {
                _data.abilityDatas.Add(new Ability(_allAbilities[i].abilityScrObj));
                continue;
            }

            IAbility abilityInterface = _allAbilities[i].GetComponent<IAbility>();
            if (abilityInterface == null) continue;
            
            for (int j = 0; j < abilityData.activationCount; j++)
            {
                abilityInterface.Activate();
            }
        }
    }
    
    
    // Indications
    private void Toggle_AbilityBar()
    {
        FoodData_Controller foodIcon = _player.foodIcon;

        bool toggle = _player.hidden == false && _abilityBar.spriteIndex > 0 
                      && _data.abilityPoint > 0 && foodIcon.hasFood == false;

        _abilityBar.Load_Custom(_maxAbilityPoint, _data.abilityPoint);
        _abilityBar.Toggle_Duration(toggle);
    }
    

    // Main Ability
    public bool AbilityPoint_Maxed()
    {
        return _data.abilityPoint >= _maxAbilityPoint;
    }

    public void Set_AbilityPoint(int setValue)
    {
        _data.Set_AbilityPoint(Mathf.Clamp(setValue, 0, _maxAbilityPoint));
    }

    private void Increase_AbilityPoint(int increaseValue)
    {
        int setValue = _data.abilityPoint + increaseValue;
        _data.Set_AbilityPoint(Mathf.Clamp(setValue, 0, _maxAbilityPoint));
        
        if (AbilityPoint_Maxed() == false) return;
        if (ActivateAvailable_AbilityScrObjs().Count <= 0) return;
        
        OnMaxPoint?.Invoke();
    }


    // Ability Data
    public List<Ability_ScrObj> All_AbilityScrObjs()
    {
        List<Ability_ScrObj> allAbilities = new();

        foreach (Ability_Behaviour ability in _allAbilities)
        {
            allAbilities.Add(ability.abilityScrObj);
        }

        return allAbilities;
    }
    
    public List<Ability_ScrObj> ActivateAvailable_AbilityScrObjs()
    {
        List<Ability> abilityDatas = _data.abilityDatas;
        List<Ability_ScrObj> abilityScrObjs = new();

        for (int i = 0; i < abilityDatas.Count; i++)
        {
            if (abilityDatas[i].ActivationCount_Maxed()) continue;
            abilityScrObjs.Add(abilityDatas[i].abilityScrObj);
        }

        return abilityScrObjs;
    }
    
    
    private Ability_Behaviour Ability_Behaviour(Ability_ScrObj abilityScrObj)
    {
        for (int i = 0; i < _allAbilities.Length; i++)
        {
            if (_allAbilities[i].abilityScrObj != abilityScrObj) continue;
            return _allAbilities[i];
        }

        return null;
    }
    
    public void Activate_Ability(Ability_ScrObj ability)
    {
        Ability activatedAbility = _data.AbilityData(ability);
        if (activatedAbility == null) return;
        
        Ability_Behaviour behaviour = Ability_Behaviour(ability);
        if (behaviour == null) return;
                
        activatedAbility.Update_ActivationCount(1);

        if (!behaviour.gameObject.TryGetComponent(out IAbility abilityInterface)) return;
        abilityInterface.Activate();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(AbilityManager))]
public class AbilityManager_Editor : Editor
{
    private SerializedProperty activateAbilityProp;

    private void OnEnable()
    {
        activateAbilityProp = serializedObject.FindProperty("editorAbility");
    }

    public override void OnInspectorGUI()
    {
        AbilityManager manager = (AbilityManager)target;

        base.OnInspectorGUI();
        serializedObject.Update();

        GUILayout.Space(60);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PropertyField(activateAbilityProp, GUIContent.none);
        Ability_ScrObj activateAbility = (Ability_ScrObj)activateAbilityProp.objectReferenceValue;

        if (GUILayout.Button("Activate Ability"))
        {
            manager.Activate_Ability(activateAbility);
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Increase Ability Point"))
        {
            AbilityManager.IncreasePoint?.Invoke(1);
        }

        GUILayout.Space(20);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif