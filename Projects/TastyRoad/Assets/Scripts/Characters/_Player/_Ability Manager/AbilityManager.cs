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

    [Space(20)]
    [SerializeField] private Ability_Behaviour[] _allAbilities;
    public Ability_Behaviour[] allAbilities => _allAbilities;
    
    [SerializeField][Range(0, 100)] private int _maxAbilityPoint;
    public int maxAbilityPoint => _maxAbilityPoint;


    private List<Ability> _abilityDatas = new();
    public List<Ability> abilityDatas => _abilityDatas;
    
    private int _currentAbilityPoint;
    public int currentAbilityPoint => _currentAbilityPoint;

    
    public static Action<Ability_ScrObj, int> IncreasePoint;
    public static Action OnPointIncrease;

    public static Action OnMaxPoint;


    // Editor
    [HideInInspector] public Ability_ScrObj editorAbility;


    // MonoBehaviour
    private void Start()
    {
        Load_AbilityDatas();

        // subscriptions
        IncreasePoint += Increase_AbilityPoint;
    }

    private void OnDestroy()
    {
        // subscriptions
        IncreasePoint -= Increase_AbilityPoint;
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("AbilityManager/_currentAbilityPoint", _currentAbilityPoint);
        ES3.Save("AbilityManager/_abilityDatas", _abilityDatas);
    }

    public void Load_Data()
    {
        _currentAbilityPoint = ES3.Load("AbilityManager/_currentAbilityPoint", _currentAbilityPoint);
        _abilityDatas = ES3.Load("AbilityManager/_abilityDatas", _abilityDatas);
    }


    // Data
    private void Load_AbilityDatas()
    {
        for (int i = 0; i < _allAbilities.Length; i++)
        {
            Ability abilityData = AbilityData(_allAbilities[i].abilityScrObj);
            if (abilityData == null)
            {
                _abilityDatas.Add(new Ability(_allAbilities[i].abilityScrObj));
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
    

    // Main Ability
    public bool AbilityPoint_Maxed()
    {
        return _currentAbilityPoint >= _maxAbilityPoint;
    }

    public void Set_AbilityPoint(int setValue)
    {
        _currentAbilityPoint = Mathf.Clamp(setValue, 0, _maxAbilityPoint);
    }
    
    /// <summary>
    /// AbilityManager.OnPointIncrease(Ability_ScrObj abilityScrObj, increase value);
    /// </summary>
    private void Increase_AbilityPoint(Ability_ScrObj abilityScrObj, int increaseValue)
    {
        Ability abilityData = AbilityData(abilityScrObj);
        abilityData.Set_AbilityPoint(abilityData.abilityPoint + increaseValue);

        if (AbilityPoint_Maxed())
        {
            if (ActivateAvailable_AbilityDatas().Count <= 0) return;
            
            OnMaxPoint?.Invoke();
            return;
        }

        _currentAbilityPoint += increaseValue;
        OnPointIncrease?.Invoke();
    }


    // Ability Data
    public List<Ability_ScrObj> ActivateAvailable_AbilityScrObjs()
    {
        List<Ability_ScrObj> abilityScrObjs = new();

        for (int i = 0; i < _abilityDatas.Count; i++)
        {
            if (_abilityDatas[i].ActivationCount_Maxed()) continue;
            abilityScrObjs.Add(_abilityDatas[i].abilityScrObj);
        }

        return abilityScrObjs;
    }
    
    public List<Ability> ActivateAvailable_AbilityDatas()
    {
        List<Ability> abilityDatas = new();

        for (int i = 0; i < _abilityDatas.Count; i++)
        {
            if (_abilityDatas[i].ActivationCount_Maxed()) continue;
            if (_abilityDatas[i].AbilityPoint_Maxed() == false) continue;
            
            abilityDatas.Add(_abilityDatas[i]);
        }
        
        abilityDatas.Sort((a, b) => b.abilityPoint.CompareTo(a.abilityPoint));
        
        return abilityDatas;
    }
    
    
    public Ability AbilityData(Ability_ScrObj targetAbility)
    {
        for (int i = 0; i < _abilityDatas.Count; i++)
        {
            if (targetAbility != _abilityDatas[i].abilityScrObj) continue;
            return _abilityDatas[i];
        }
        return null;
    }
    
    public void Activate_Ability(Ability_ScrObj ability)
    {
        Ability activatedAbility = AbilityData(ability);
                
        activatedAbility.Update_ActivationCount(1);
        activatedAbility.Set_AbilityPoint(0);
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
            AbilityManager.IncreasePoint?.Invoke(activateAbility, 1);
        }

        GUILayout.Space(20);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif