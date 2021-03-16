using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionFoundPlayer : Condition
{
    SeekerEnemyController enemyController;

    public ConditionFoundPlayer(SeekerEnemyController eC)
    {
        enemyController = eC;
    }

    public override bool Test()
    {
        return enemyController.foundPlayer;
    }
}
