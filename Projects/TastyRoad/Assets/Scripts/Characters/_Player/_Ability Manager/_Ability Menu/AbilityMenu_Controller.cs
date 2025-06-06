using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class AbilityMenu_Controller : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Input_Manager _inputManager;
    [SerializeField] private InfoTemplate_Trigger _infoTemplate;

    [Space(20)] 
    [SerializeField] private GameObject _abilityMenuPanel;
    [SerializeField] private AbilityMenu_Button[] _buttons;
    
    [Space(20)] 
    [SerializeField] private Sprite _emptyIconSprite;
    
    private int _buttonIndexNum;


    // MonoBehaviour
    private void Start()
    {
        Toggle_Menu();
        
        // subscriptions
        AbilityManager.OnMaxPoint += Toggle_Menu;
        // PauseExit += Toggle_Menu; //
        
        _inputManager.OnCursorControl += Navigate_Button;
        _inputManager.OnHoldSelect += Select_Ability;
    }

    private void OnDestroy()
    {
        // subscriptions
        AbilityManager.OnMaxPoint -= Toggle_Menu;
        // PauseExit -= Toggle_Menu; //

        _inputManager.OnCursorControl -= Navigate_Button;
        _inputManager.OnHoldSelect -= Select_Ability;
    }
    
    
    // Buttons
    private void Update_AbilityButtons()
    {
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        List<Ability> availableAbilities = manager.ActivateAvailable_AbilityDatas();

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (availableAbilities.Count <= 0)
            {
                _buttons[i].Empty_AbilityIndication(_emptyIconSprite, _infoTemplate.TemplateString(0));
                continue;
            }

            _buttons[i].Set_AbilityIndication(availableAbilities[0]);
            availableAbilities.RemoveAt(0);
        }
    }
    
    private void Navigate_Button(Vector2 navigateDirection)
    {
        if (navigateDirection.y == 0) return;
        
        _buttonIndexNum += (int)navigateDirection.y * -1;
        _buttonIndexNum = (_buttonIndexNum + _buttons.Length) % _buttons.Length;

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (i == _buttonIndexNum)
            {
                _buttons[i].selectIndicator.SetActive(true);
                continue;
            }
            _buttons[i].selectIndicator.SetActive(false);
        }
    }
    
    
    // Menu
    private void Toggle_Menu(bool toggle)
    {
        _abilityMenuPanel.SetActive(toggle);
        _inputManager.Toggle_Input(toggle);

        if (toggle == false) return;

        foreach (AbilityMenu_Button button in _buttons)
        {
            button.selectIndicator.SetActive(false);
        }

        _buttonIndexNum = 0;
        _buttons[0].selectIndicator.SetActive(true);
    }
    
    private void Toggle_Menu()
    {
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;

        if (manager.AbilityPoint_Maxed() == false || manager.ActivateAvailable_AbilityDatas().Count <= 0)
        {
            Toggle_Menu(false);
            return;
        }
        
        Toggle_Menu(true);
        Update_AbilityButtons();
    }
    
    
    private void Select_Ability()
    {
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        Ability_ScrObj currentAbility = _buttons[_buttonIndexNum].abilityScrObj;

        // random ability activation
        if (currentAbility == null)
        {
            List<Ability_ScrObj> abilities = manager.ActivateAvailable_AbilityScrObjs();
            currentAbility = abilities[Random.Range(0, abilities.Count)];
        }

        manager.Activate_Ability(currentAbility);
        Toggle_Menu(false);

        // dialog
        Ability abilityData = manager.AbilityData(currentAbility);
        
        int activationCount = abilityData.activationCount;
        Sprite abilitySprite = currentAbility.ProgressIcon(activationCount);

        string abilityInfo = currentAbility.abilityName + "\n\n";
        string activationInfo = activationCount + "/" + currentAbility.Max_ActivationCount() + " ";

        DialogData dialogData = new(abilitySprite, abilityInfo + activationInfo + "leveled up");
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(dialogData);
    }
}
