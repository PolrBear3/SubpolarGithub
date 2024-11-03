using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public interface IAbility
{
    void Activate();
}

public class AbilityManager : MonoBehaviour, ISaveLoadable
{
    [Header("")]
    [SerializeField] private Player_Controller _player;
    public Player_Controller player => _player;

    [Header("")]
    [SerializeField] private Ability_Behaviour[] _allAbilities;

    private List<Ability> _activatedAbilities = new();
    public List<Ability> activatedAbilities => _activatedAbilities;


    // Editor
    [HideInInspector] public Ability_ScrObj editorAbility;


    // MonoBehaviour
    private void Start()
    {
        Load_ActivatedAbilities();
    }


    // ISaveLoadable
    public void Save_Data()
    {
        ES3.Save("AbilityManager/_activatedAbilities", _activatedAbilities);
    }

    public void Load_Data()
    {
        _activatedAbilities = ES3.Load("AbilityManager/_activatedAbilities", _activatedAbilities);
    }


    // Data Control
    private void Load_ActivatedAbilities()
    {
        for (int i = 0; i < _allAbilities.Length; i++)
        {
            if (Ability_Activated(_allAbilities[i].abilityScrObj) == false) continue;

            IAbility abilityInterface = _allAbilities[i].GetComponent<IAbility>();

            for (int j = 0; j < Activated_Ability(_allAbilities[i].abilityScrObj).activationCount; j++)
            {
                abilityInterface.Activate();
            }
        }
    }


    //
    private Ability Activated_Ability(Ability_ScrObj targetAbility)
    {
        for (int i = 0; i < _activatedAbilities.Count; i++)
        {
            if (targetAbility != _activatedAbilities[i].abilityScrObj) continue;
            return _activatedAbilities[i];
        }
        return null;
    }

    private bool Ability_Activated(Ability_ScrObj checkAbility)
    {
        for (int i = 0; i < _activatedAbilities.Count; i++)
        {
            if (checkAbility != _activatedAbilities[i].abilityScrObj) continue;
            return true;
        }
        return false;
    }


    public void Activate_Ability(Ability_ScrObj ability)
    {
        for (int i = 0; i < _allAbilities.Length; i++)
        {
            if (ability != _allAbilities[i].abilityScrObj) continue;

            _allAbilities[i].GetComponent<IAbility>().Activate();

            if (Ability_Activated(ability))
            {
                Activated_Ability(ability).Update_ActivationCount(1);
                return;
            }

            _activatedAbilities.Add(new(ability));
            return;
        }
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
        GUILayout.Space(20);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif