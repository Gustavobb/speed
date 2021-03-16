using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionEndLaserAttack : Condition
{
    AttackState attackState;

    public ConditionEndLaserAttack(AttackState As)
    {
        attackState = As;
    }

    public override bool Test()
    {
        return attackState.finishedShooting;
    }
}
