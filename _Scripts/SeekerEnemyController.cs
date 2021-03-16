using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerEnemyController : SteerableBehaviour, IShooter, IDamageable
{
    public Transform playerTransform;
    public Vector3 playerOldPosition;
    RaycastHit2D hit;
    float angle;
    public bool foundPlayer = false;
    public LineRenderer lineRenderer;
    public Transform firePoint;
    public GameObject spriteObject, startVFX, endVFX;
    public LayerMask layerMask;
    public SpriteRenderer dissolveRenderer;
    public GameManeger gm;
    public AudioSource deathsound;
    int lifes = 5;
    bool dead = false;

    List<ParticleSystem> particles = new List<ParticleSystem>();

    void Start()
    {
        playerTransform = GameObject.FindWithTag("_Player").transform;
        FillLists();
        DisableLaser();
    }

    public void Shoot()
    {
        CastRay(playerOldPosition);
        UpdateLaser();
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
            DisableLaser();
            GetComponent<AudioSource>().Stop();
            spriteObject.GetComponent<SpriteRenderer>().enabled = false;
            dissolveRenderer.enabled = true;
            dead = true;
            foundPlayer = false;
            deathsound.Play();
            StartCoroutine(Wait(1f));
        }
    }

    public void EnableLaser()
    {
        GetComponent<AudioSource>().Play();
        lineRenderer.enabled = true;
        for (int i = 0; i < particles.Count; i++) particles[i].Play();
    }

    public void UpdateLaser()
    {
        if (!dead)
        {
            lineRenderer.SetPosition(0, (Vector2)firePoint.position);
            startVFX.transform.position = (Vector2)firePoint.position;

            if (hit) 
            {
                lineRenderer.SetPosition(1, hit.point);
                if (hit.collider.tag == "_Player" && !dead) 
                {
                    SteerableBehaviour steerable = hit.transform.gameObject.GetComponent<SteerableBehaviour>();
                    IDamageable Idamageable = steerable as IDamageable;
                    if(Idamageable != null)
                    {
                        Idamageable.TakeDamage();
                    }
                }
            }

            endVFX.transform.position = lineRenderer.GetPosition(1);
        }
    }

    public void DisableLaser()
    {
        lineRenderer.enabled = false;
        for (int i = 0; i < particles.Count; i++) particles[i].Stop();
    }

    public void SeekPlayer()
    {
        if (!dead && gm.start && playerTransform != null)
        {
            float playerX = playerTransform.position.x;
            float playerY = playerTransform.position.y;

            playerX -= transform.position.x;
            playerY -= transform.position.y;

            angle = Mathf.Atan2(playerY, playerX);

            startVFX.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle * Mathf.Rad2Deg));
            endVFX.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle * Mathf.Rad2Deg));
            spriteObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle * Mathf.Rad2Deg - 90));

            CastRay(playerTransform.position);
            if (hit.collider.tag == "_Player") foundPlayer = true;
        }
    }

    public void CastRay(Vector3 t)
    {
        Vector2 direction = (Vector2) t - (Vector2) transform.position;
        hit = Physics2D.Raycast((Vector2) transform.position, direction.normalized, 100f, layerMask);
    }

    void FillLists()
    {
        particles.Add(startVFX.transform.GetChild(0).GetComponent<ParticleSystem>());
        particles.Add(endVFX.transform.GetChild(0).GetComponent<ParticleSystem>());
        particles.Add(endVFX.transform.GetChild(1).GetComponent<ParticleSystem>());
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
