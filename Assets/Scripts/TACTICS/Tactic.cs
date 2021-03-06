using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[InlineEditor]
public class Tactic : MonoBehaviour
{
    public bool isTurnedOn;                                                 // Is this Gambit activated?

    public BattleCharacterController _Performer;                            // Who is performing Action
    public BattleCharacterController _Target;                               // Who is target of Action

    public List<Action> _Actions;                                            // What is to be done
    public Condition _Condition;                                            // What needs to be met to perform Action

    public bool ConditionIsMet;                                             // Is condition met?

    public CharacterType RetrieveTargetType()
    {
        return _Condition.targetType;                  // Either CHARACTER or ENEMY
    }
    public CharacterActiveStatus RetrieveTargetStatus()
    {
        return _Condition.targetStatus;                // Either ACTIVE or DOWNED
    }
    public void CallCheck()
    {
       ConditionIsMet = _Condition.ConditionCheck(_Target); // Runs the condition, returns true/false
    }
}
