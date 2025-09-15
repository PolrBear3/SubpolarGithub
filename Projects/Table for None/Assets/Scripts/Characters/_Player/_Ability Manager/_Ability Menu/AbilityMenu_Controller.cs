using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Serialization;

public class AbilityMenu_Controller : MonoBehaviour
{
    [Space(20)]
    [SerializeField] private Input_Manager _inputManager;
    [SerializeField] private InfoTemplate_Trigger _infoTemplate;
    
    [Space(20)] 
    [SerializeField] private GameObject _toggleMenu;
    public GameObject toggleMenu => _toggleMenu; 
    
    [SerializeField] private AbilityMenu_Button[] _buttons;
    
    [Space(20)] 
    [SerializeField] private UI_EffectController _uiEffectController;
    [SerializeField] private Image _menuPanel;
    
    [Space(10)]
    [SerializeField] private Sprite _emptyIcon;
    
    [Space(20)] 
    [SerializeField] private Color _blurColor;
    [SerializeField][Range(0, 10)] private float _transitionDuration;
    
    
    private int _buttonIndexNum;
    private bool _buttonsUpdated;


    // MonoBehaviour
    private void Start()
    {
        Load_Data();
        Toggle_Menu();
        
        // subscriptions
        AbilityManager.OnMaxPoint += Toggle_Menu;
        
        PauseMenu_Controller pauseMenu = PauseMenu_Controller.instance;
        
        _inputManager.OnExit += () => Toggle_Menu(false);
        _inputManager.OnExit += () => pauseMenu.Toggle_Pause(true);

        pauseMenu.OnPauseExit += Toggle_Menu;

        _inputManager.OnCursorControl += Navigate_Button;
        _inputManager.OnHoldSelect += Select_Ability;
        
        _inputManager.OnSelectStart += Hold_CurrentButton;
        _inputManager.OnSelect += Release_CurrentButton;
        _inputManager.OnHoldSelect += Release_CurrentButton;

        Localization_Controller.instance.OnLanguageChanged += UpdateCurrent_AbilityButtons;
    }

    private void OnDestroy()
    {
        // subscriptions
        AbilityManager.OnMaxPoint -= Toggle_Menu;
        
        PauseMenu_Controller.instance.OnPauseExit -= Toggle_Menu;

        _inputManager.OnCursorControl -= Navigate_Button;
        _inputManager.OnHoldSelect -= Select_Ability;
        
        _inputManager.OnSelectStart -= Hold_CurrentButton;
        _inputManager.OnSelect -= Release_CurrentButton;
        _inputManager.OnHoldSelect -= Release_CurrentButton;
        
        Localization_Controller.instance.OnLanguageChanged -= UpdateCurrent_AbilityButtons;
    }

    private void OnApplicationQuit()
    {
        Save_Data();
    }
    
    
    // ISaveLoadable
    private void Save_Data()
    {
        if (_buttonsUpdated == false) return;

        List<Ability> buttonAbilities = new();
        
        foreach (AbilityMenu_Button button in _buttons)
        {
            buttonAbilities.Add(new(button.abilityScrObj));
        }
        
        ES3.Save("AbilityMenu_Controller/List<Ability>", buttonAbilities);
    }

    private void Load_Data()
    {
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        
        if (manager.AbilityPoint_Maxed() == false) return;
        _buttonsUpdated = true;
        
        List<Ability> buttonAbilities = ES3.Load("AbilityMenu_Controller/List<Ability>", new List<Ability>());

        for (int i = 0; i < buttonAbilities.Count; i++)
        {
            if (buttonAbilities[i].abilityScrObj == null)
            {
                _buttons[i].Empty_AbilityIndication(_emptyIcon, _infoTemplate.TemplateString(0));
                continue;
            }
            _buttons[i].Set_AbilityIndication(buttonAbilities[i]);
        }
    }
    
    
    // Buttons
    private void Update_AbilityButtons()
    {
        _buttonsUpdated = true;
        
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        AbilityManager_Data managerData = manager.data;
        
        List<Ability_ScrObj> availableAbilities = manager.ActivateAvailable_AbilityScrObjs();
        List<float> abilityWeights = new();

        float totalWeight = 0;

        foreach (Ability_ScrObj ability in availableAbilities)
        {
            int activationCount = managerData.Ability_ActivationCount(ability);
            
            float ratio = activationCount / (float)ability.Max_ActivationCount();
            float weight = Mathf.Clamp01(1f - ratio);

            abilityWeights.Add(weight);
            totalWeight += weight;
        }

        foreach (AbilityMenu_Button button in _buttons)
        {
            // not enough abilities to upgrade
            if (availableAbilities.Count <= 0)
            {
                button.Empty_AbilityIndication(_emptyIcon, _infoTemplate.TemplateString(0));
                continue;
            }
            
            float randValue = UnityEngine.Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;

            for (int i = 0; i < abilityWeights.Count; i++)
            {
                cumulativeWeight += abilityWeights[i];
                
                if (randValue > cumulativeWeight) continue;

                Ability setAbility = managerData.AbilityData(availableAbilities[i]);
                button.Set_AbilityIndication(setAbility);

                totalWeight -= abilityWeights[i];
                
                abilityWeights.RemoveAt(i);
                availableAbilities.RemoveAt(i);

                break;
            }
        }
    }

    private void UpdateCurrent_AbilityButtons()
    {
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        
        if (manager.AbilityPoint_Maxed() == false) return;
        _buttonsUpdated = true;
        
        for (int i = 0; i < _buttons.Length; i++)
        {
            Ability_ScrObj setAbility = _buttons[i].abilityScrObj;

            if (setAbility == null)
            {
                _buttons[i].Empty_AbilityIndication(_emptyIcon, _infoTemplate.TemplateString(0));
                continue;
            }
            
            _buttons[i].Set_AbilityIndication(manager.data.AbilityData(setAbility));
        }
    }


    private void Navigate_Button(int buttonIndex)
    {
        _buttonIndexNum = (buttonIndex + _buttons.Length) % _buttons.Length;
        
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (i == _buttonIndexNum)
            {
                _buttons[i].Toggle_Select(true);
                continue;
            }
            _buttons[i].Toggle_Select(false);
        }
    }
    private void Navigate_Button(Vector2 navigateDirection)
    {
        if (navigateDirection.y == 0) return;
        Navigate_Button(_buttonIndexNum + (int)navigateDirection.y * -1);
    }

    
    private void Hold_CurrentButton()
    {
        AbilityMenu_Button currentButton = _buttons[_buttonIndexNum];
        UI_ClockTimer holdTimer = currentButton.holdTimer;
        
        holdTimer.Run_ClockSprite();
    }

    private void Release_CurrentButton()
    {
        AbilityMenu_Button currentButton = _buttons[_buttonIndexNum];
        UI_ClockTimer holdTimer = currentButton.holdTimer;
        
        holdTimer.Stop_ClockSpriteRun();
    }
    
    
    // Menu
    private void Toggle_Menu(bool toggle)
    {
        if (PauseMenu_Controller.instance.isPaused) return;
        Main_Controller.instance.transitionCanvas.Toggle_PauseScreen(toggle);

        _toggleMenu.SetActive(toggle);
        _inputManager.Toggle_Input(toggle);
        
        GlobalLight_Controller lightController = Main_Controller.instance.globalLightController;

        if (toggle == false)
        {
            lightController.Toggle_CurrentColorLock(toggle);
            lightController.Update_CurrentColor(lightController.defaultColor, _transitionDuration);

            Save_Data();
            return;
        }
        
        lightController.Update_CurrentColor(_blurColor, _transitionDuration);
        lightController.Toggle_CurrentColorLock(toggle);

        foreach (AbilityMenu_Button button in _buttons)
        {
            button.Toggle_Select(false);
            button.holdTimer.Stop_ClockSpriteRun();
        }

        _buttonIndexNum = 0;
        _buttons[0].Toggle_Select(true);
    }
    
    public void Toggle_Menu()
    {
        if (Input_Controller.instance.Current_ActionMapNum() != 0) return;
        
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        if (manager.AbilityPoint_Maxed() == false || manager.ActivateAvailable_AbilityScrObjs().Count <= 0)
        {
            Toggle_Menu(false);
            return;
        }

        if (_toggleMenu.activeSelf) return;
        
        Toggle_Menu(true);
        _uiEffectController.Update_Scale(_menuPanel.rectTransform);

        if (_buttonsUpdated)
        {
            UpdateCurrent_AbilityButtons();
            return;
        }
        Update_AbilityButtons();
    }
    
    
    private void Select_Ability()
    {
        _buttonsUpdated = false;
        
        AbilityManager manager = Main_Controller.instance.Player().abilityManager;
        Ability_ScrObj currentAbility = _buttons[_buttonIndexNum].abilityScrObj;

        manager.Set_AbilityPoint(0);
        Toggle_Menu(false);
        
        if (currentAbility == null) return;
        manager.Activate_Ability(currentAbility);

        // dialog
        int activationCount = manager.data.Ability_ActivationCount(currentAbility);
        
        Sprite abilitySprite = currentAbility.activationIconSprite[activationCount];
        string activationInfo = activationCount + "/" + currentAbility.Max_ActivationCount() + " " + _infoTemplate.TemplateString(1);

        DialogData dialogData = new(abilitySprite, activationInfo);
        gameObject.GetComponent<DialogTrigger>().Update_Dialog(dialogData);
    }
    
    /// <summary>
    /// Menu Button OnPointer Click
    /// </summary>
    public void Select_Ability(int buttonIndex)
    {
        if (buttonIndex != _buttonIndexNum)
        {
            Navigate_Button(buttonIndex);
            return;
        }
        Select_Ability();
    }
}
