using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHeroController : BattleCharacterController
{
    // VARIABLES
    [SerializeField]
    internal HeroExtension myHero;
    private string currentAnimState;
    internal bool isPerformingActions;

    const string Battle_EnterFight = "Enter Fight";
    const string Battle_Stance = "Stance";
    const string Battle_Die = "Die";

    // UPDATES
    private void OnEnable()
    {
        BattleStateMachine.OnNewStateSwitched += NewCombatState;
    }
    private void OnDisable()
    {
        BattleStateMachine.OnNewStateSwitched -= NewCombatState;
    }

    // METHODS
    #region Standard Action Methods
    internal IEnumerator DoManualAction(List<Action> chosenActions, BattleCharacterController target, int targetList)
    {
        // Play Anim
        if (myHero._CurrentHP > 0)
        {
            isPerformingActions = true;
            for (int i = 0; i < chosenActions.Count; i++)
            {
                if (chosenActions[i] != null)
                {
                    if (myHero.myTacticController.CheckIfStillInList(target, targetList))
                    {
                        // DO EACH IN SEQUENCE (ONE AFTER THE OTHER)
                        yield return DoActionSegment(chosenActions[i], target, targetList, i);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            yield return null;
        }
        isPerformingActions = false;
        myHero.myTacticController.ChosenActions = null;
        myHero.myTacticController.ChosenTarget = null;
        myHero.myTacticController.ManualActionInput = false;
    }
    internal IEnumerator DoTacticAction(Tactic _TacticToPerform, int targetList)
    {
        // Play Anim Here
        if (myHero._CurrentHP > 0)
        {
            isPerformingActions = true;
            for(int i = _TacticToPerform._Actions.Count - 1; i >= 0; i--)
            {
                if (_TacticToPerform._Actions[i] != null)
                {
                    if (myHero.myTacticController.CheckIfStillInList(_TacticToPerform._Target, targetList))
                    {
                        // DO EACH IN SEQUENCE (ONE AFTER THE OTHER)
                        yield return DoActionSegment(_TacticToPerform._Actions[i], _TacticToPerform._Target, targetList, i);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            yield break;
        }
        isPerformingActions = false;
        _TacticToPerform._Target = null;
        myHero.myTacticController.ChosenActions = null;
        myHero.myTacticController.ChosenTarget = null;
    }
    private IEnumerator DoActionSegment(Action action, BattleCharacterController target, int targetList, int ActionPosition)
    {
        myHero._ActionChargeAmount -= 25 * action._SegmentCost;
        ChangeAnimationState(action.AnimationName);
        
        yield return new WaitForSeconds(action.AnimationTiming);
        if (myHero.myTacticController.CheckIfStillInList(target, targetList))
        {
            if (action.ActionType == ActionType.ITEM)
            {
                Consumable consumable = GameManager._instance._ConsumablesDatabase.Find(x => x.myAction == action);
                ItemCapsule itemCapsule = InventoryManager._instance.ConsumablesInBag.Find(x => x.thisItem == consumable);

                if (itemCapsule != null)
                {
                    InventoryManager._instance.RemoveFromInventory(itemCapsule);
                }
                else
                {
                    yield return null;
                }
            }
            foreach (ActionBehaviour aBehaviour in action.Behaviours)
            {
                aBehaviour.PreActionTargetting(this, action, target);
            }
        }
        myHero.myTacticController.ChosenActions[ActionPosition] = null;
        yield return new WaitForSeconds(1f);
    }
    #endregion
    #region Basic Commands
    public override void ActiveStateBehaviour()
    {
        if (!isPerformingActions)
        {
            myHero._ActionChargeAmount += myHero._ActionRechargeSpeed * Time.deltaTime;
            myHero._ActionChargeAmount = Mathf.Clamp(myHero._ActionChargeAmount, 0, 100);
            myHero.myTacticController.SetNextAction();
        }
    }
    public void Revivecheck()
    {
        if (myHero._CurrentHP > 0)
        {
            BattleStateMachine._HeroesActive.Add(this);
            BattleStateMachine._HeroesDowned.Remove(this);
            myMovementController.agent.isStopped = false;
            myMovementController.isRoaming = true;
        }
    }
    public override void DieCheck()
    {
        if (myHero._CurrentHP <= 0)
        {
            FindObjectOfType<BattleStateMachine>().CheckCharIsDead(this);
            myHero._CurrentHP = 0;
            myHero._ActionChargeAmount = 0;
            ChangeAnimationState(Battle_Die);
            myMovementController.agent.isStopped = true;
            myMovementController.isRoaming = false;
        }
    }
    #endregion

    private void NewCombatState(CombatState combatState)
    {
        switch(combatState)
        {
            case CombatState.START:
                if (myHero._CurrentHP > 0)
                {
                    ChangeAnimationState(Battle_EnterFight);
                }
                else
                {
                    ChangeAnimationState(Battle_Die);
                }
                break;

            case CombatState.ACTIVE:
                if (myHero._CurrentHP > 0)
                {
                    ChangeAnimationState(Battle_Stance);
                    myMovementController.isRoaming = true;
                }
                else
                {
                    myMovementController.isRoaming = false;
                }
                break;

            case CombatState.WAIT:
                if (myHero._CurrentHP > 0)
                {
                    ChangeAnimationState(Battle_Stance);
                    myMovementController.isRoaming = false;
                }
                break;

            case CombatState.END:
                myMovementController.isRoaming = false;
                break;
        }
    }
    private void ChangeAnimationState(string newAnimState)
    {
        animator.Play(newAnimState);
        currentAnimState = newAnimState;
    }
}
