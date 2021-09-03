using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HS_Detective : HeroExtension
{
    protected override void Awake()
    {
        base.Awake();
        AssignStats();
        Debug.Log(this.name + " Activated");
    }
    public override void AssignStats()
    {
        _MaxHP = (int)(_CSA._BaseHP * (1 + _GrowthRateStrong) * _Level) + equipHP;
        _MaxMP = (int)(_CSA._BaseMP * (1 + _GrowthRateStrong) * _Level) + equipMP;
        _Attack = (int)(_CSA._BaseAttack * (1 + _GrowthRateStrong) * _Level) + equipAttack;
        _MagAttack = (int)(_CSA._BaseMagAttack * (1 + _GrowthRateStrong) * _Level) + equipMagAttack;
        _Defense = (int)(_CSA._BaseDefense * (1 + _GrowthRateStrong) * _Level) + equipDefense;
        _MagDefense = (int)(_CSA._BaseMagDefense * (1 + _GrowthRateStrong) * _Level) + equipMagDefense;

        _ActionRechargeSpeed = 20;
        _CurrentHP = _MaxHP;
        _CurrentMP = _MaxMP;
    }
}