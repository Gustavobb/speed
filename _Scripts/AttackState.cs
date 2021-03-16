using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    SteerableBehaviour steerable;
    public float shootDelay = 1.0f;
    bool waitedToShoot = false;
    public bool finishedShooting = false;
    private float _lastShootTimestamp = 0.0f;
    IShooter shooter;
    SeekerEnemyController enemyController;

    public override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<SeekerEnemyController>();
        Transition seeking = new Transition();
        seeking.condition = new ConditionEndLaserAttack(this);
        seeking.target = GetComponent<SeekState>();
        transitions.Add(seeking);


        steerable = GetComponent<SteerableBehaviour>();
        shooter = steerable as IShooter;
        if(shooter == null)
        {
            throw new MissingComponentException("this GameObject does not implement IShooter");
        }
    }

    public virtual void OnEnable()
    {
        enemyController.spriteObject.GetComponent<SpriteRenderer>().color = new Color(1f, .7f, .7f, 1f);
        StartCoroutine(WaitToShoot(1f));
    }

    public virtual void OnDisable()
    {
        enemyController.spriteObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        enemyController.foundPlayer = false;
        enemyController.DisableLaser();
        waitedToShoot = false;
        finishedShooting = false;
    }

    public override void Update()
    {
        if (waitedToShoot) shooter.Shoot();
    }

    IEnumerator WaitToShoot(float time)
    {
        yield return new WaitForSeconds(time);
        enemyController.EnableLaser();
        waitedToShoot = true;
        StartCoroutine(ShootFor(3f));
    }

    IEnumerator ShootFor(float time)
    {
        yield return new WaitForSeconds(time);
        finishedShooting = true;
    }
}
