using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekState : State
{
    SteerableBehaviour steerable;
    SeekerEnemyController enemyController;

    public override void Awake()
    {
        base.Awake();

        Transition attacking = new Transition();
        enemyController = GetComponent<SeekerEnemyController>();
        attacking.condition = new ConditionFoundPlayer(enemyController);
        attacking.target = GetComponent<AttackState>();
        transitions.Add(attacking);
        steerable = GetComponent<SteerableBehaviour>();
    }

    public virtual void OnDisable()
    {
        if (enemyController.playerTransform != null) enemyController.playerOldPosition = enemyController.playerTransform.position;
    }

    public override void Update()
    {
        enemyController.SeekPlayer();
    }
}
