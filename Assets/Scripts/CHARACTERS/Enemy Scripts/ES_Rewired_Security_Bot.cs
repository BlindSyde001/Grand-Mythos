using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Rewired_Security_Bot : EnemyExtension
{
    // Smacks hard. Is weak to Lightning based attacks.

    // VARIABLES

    // METHODS
    public void ActiveStateBehaviour()
    {
        if (_ActionChargeAmount == 100)
        {
            BasicAttack();
        }

    }
    private void BasicAttack()
    {
        if (CheckForHeroTarget())
        {
            Debug.Log(this.name + " Has Attacked!");
            int x = Random.Range(0, BattleStateMachine._HeroesActive.Count);
            PerformEnemyAction(_BasicAttack, BattleStateMachine._HeroesActive[x]);
        }
    }
}
