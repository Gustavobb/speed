using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FollowerEnemyBehaviour : SteerableBehaviour, IShooter, IDamageable
{
    Transform playerTransform;
    RaycastHit2D hit;
    float angle;
    float nextWaypointDistance = 3f;
    float speed = 400f;
    public Transform firePoint;
    public GameObject spriteObject, bullet;
    public LayerMask layerMask;
    public SpriteRenderer dissolveRenderer;
    public AudioSource deathsound;
    int lifes = 2;
    public float shootDelay = 1.0f;
    float _lastShootTimestamp = 0.0f;
    public GameManeger gm;
    public ParticleSystem p1, p2;
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    bool dead = false;

    Seeker seeker;
    
    void Start()
    {
        p1.Stop();
        p2.Stop();
        playerTransform = GameObject.FindWithTag("_Player").transform;
        seeker = GetComponent<Seeker>();

        InvokeRepeating("UpdatePath", 0, .5f);
    }

    void FixedUpdate()
    {
        if (dead || !gm.start) speed = 0f;
        else 
        {
            speed = 400f;
            SeekPlayer();
        }

        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count) 
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        _ThrustForce(force);
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint ++;
        }
    }

    void UpdatePath() 
    {
        if (playerTransform != null && seeker.IsDone() && Vector2.Distance(transform.position, playerTransform.position) <= 30) seeker.StartPath(rb.position, playerTransform.position, OnPathDone);
    }

    void OnPathDone(Path p) 
    {
        if (!p.error) 
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void Shoot()
    {
        if (Time.time - _lastShootTimestamp < shootDelay) return;
        GetComponent<AudioSource>().Play();
        p1.Play();
        p2.Play();
        _lastShootTimestamp = Time.time;

        Instantiate(bullet, firePoint.position, Quaternion.identity);
    }

    public void TakeDamage()
    {
        lifes --;

        if (lifes <= 0) Die();
    }

    public void Die()
    {
        if (!dead)
        {
            spriteObject.GetComponent<SpriteRenderer>().enabled = false;
            dissolveRenderer.enabled = true;
            dead = true;
            p1.Stop();
            p2.Stop();
            deathsound.Play();
            StartCoroutine(Wait(1f));
        }
    }

    public void SeekPlayer()
    {
        if (playerTransform != null)
        {
            float playerX = playerTransform.position.x;
            float playerY = playerTransform.position.y;

            playerX -= transform.position.x;
            playerY -= transform.position.y;

            angle = Mathf.Atan2(playerY, playerX);

            spriteObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle * Mathf.Rad2Deg - 90));

            CastRay(playerTransform.position);
            if (hit.collider.tag == "_Player" && Vector2.Distance(transform.position, playerTransform.position) <= 30) Shoot();
            else 
            {
                p1.Stop();
                p2.Stop();
            }
        }
    }

    public void CastRay(Vector3 t)
    {
        Vector2 direction = (Vector2) t - (Vector2) transform.position;
        hit = Physics2D.Raycast((Vector2) transform.position, direction.normalized, 100f, layerMask);
    }

    IEnumerator Wait(float time)
    {
        Material material = dissolveRenderer.material;

        while (time > 0f)
        {
            time -= Time.deltaTime;
            material.SetFloat("_Fade", time);
            yield return null;
        }

        Destroy(gameObject);
    }
}
