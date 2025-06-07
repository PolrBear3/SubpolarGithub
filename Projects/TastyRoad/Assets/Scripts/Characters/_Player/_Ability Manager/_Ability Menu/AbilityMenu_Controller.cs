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
    
    [Space(20)] 
    [SerializeField] private Color _blurColor;
    [SerializeField][Range(0, 10)] private float _transitionDuration;
    
    
    private int _buttonIndexNum;


    // MonoBehaviour
    private void Start()
    {
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
        if (Input_Controller.instance.isHolding) return;
        if (navigateDirection.y == 0) return;
        
        _buttonIndexNum += (int)navigateDirection.y * -1;
        _buttonIndexNum = (_buttonIndexNum + _buttons.Length) % _buttons.Length;

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
        if (Input_Controller.instance.isHolding) return;
        
        TransitionCanvas_Controller.instance.Toggle_PauseScreen(toggle);
        
        _abilityMenuPanel.SetActive(toggle);
        _inputManager.Toggle_Input(toggle);
        
        GlobalLight_Controller lightController = Main_Controller.instance.globalLightController;

        if (toggle == false)
        {
            lightController.Toggle_CurrentColorLock(toggle);
            lightController.Update_CurrentColor(lightController.defaultColor, _transitionDuration);
            
            return;
        }
        
        lightController.Update_CurrentColor(_blurColor, _transitionDuration);
        lightController.Toggle_CurrentColorLock(toggle);

        foreach (AbilityMenu_Button button in _buttons)
        {
            button.Toggle_Select(false);
        }

        _buttonIndexNum = 0;
        _buttons[0].Toggle_Select(true);
    }
    
    public void Toggle_Menu()
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

        manager.Set_AbilityPoint(0);
        Toggle_Menu(false);
        
        if (currentAbility == null) return;
        manager.Activate_Ability(currentAbility);

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
