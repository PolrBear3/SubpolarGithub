using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMagnet_Ability : Ability_Behaviour, IAbility
{
    [Space(20)]
    [SerializeField][Range(0, 10)] private float _multiplyIncreaseAmount;
    
    
    // IAbility
    public void Activate()
    {
        GoldSystem goldSystem = GoldSystem.instance;
        
        float currentMultiply = goldSystem.data.bonusMultiplyAmount;
        goldSystem.data.Set_BonusMultiplyAmount(currentMultiply + _multiplyIncreaseAmount);
    }
}
