using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CndStatusAfflicted : Condition
{
    public bool isBlinded;
    public bool isSilenced;
    public bool isFurored;
    public bool isParalysed;
    public bool isPhysDown;
    public bool isMagDown;
    public override bool ConditionCheck(BattleCharacterController target)
    {
        CharacterTemplate tempToUse;
        switch (target.myType)
        {
            case BattleCharacterController.ControllerType.HERO:
                {
                    BattleHeroController a = target as BattleHeroController;
                    tempToUse = a.myHero;
                    break;
                }

            default:
                {
                    BattleEnemyController a = target as BattleEnemyController;
                    tempToUse = a.myEnemy;
                    break;
                }
        }

        if (isBlinded)
        {
            return tempToUse._IsBlinded;
        }
        if (isSilenced)
        {
            return tempToUse._IsSilenced;
        }
        if (isFurored)
        {
            return tempToUse._IsFurored;
        }
        if (isParalysed)
        {
            return tempToUse._IsParalysed;
        }
        if (isPhysDown)
        {
            return tempToUse._IsPhysDown;
        }
        if (isMagDown)
        {
            return tempToUse._IsMagDown;
        }
        else return false;
    }
}
