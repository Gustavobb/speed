using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SteerableBehaviour, IShooter, IDamageable
{
    int lifes;
    bool move;
    float angle;
    float mousePosX;
    float mousePosY;
    Vector3 targetPos;

    public GameObject bullet, spriteObject;
    public Transform gunPoint;
    public ParticleSystem p1, p2;
    public SpriteRenderer dissolveRenderer;
    public float shootDelay = 1.0f;
    private float _lastShootTimestamp = 0.0f;
    bool die = false;
    public AudioSource deathsound;

    private void Start()
    {
        p1.Stop();
        p2.Stop();
        lifes = 1;
    }
        
    public void TakeDamage()
    {
        lifes--;
        if (lifes <= 0) Die();
    }

    public void Shoot()
    {
        if (Time.time - _lastShootTimestamp < shootDelay) return;
        p1.Play();
        GetComponent<AudioSource>().Play();
        p2.Play();
        _lastShootTimestamp = Time.time;
        Instantiate(bullet, gunPoint.position, Quaternion.identity);
    }

    public void Die()
    {
        if (!die)
        {
            spriteObject.GetComponent<SpriteRenderer>().enabled = false;
            dissolveRenderer.enabled = true;
            die = true;
            p1.Stop();
            p2.Stop();
            deathsound.Play();
            StartCoroutine(Wait(1f));
        }
    }

    void FixedUpdate()
    {
        if (die) return;
        move = Input.GetKey("w");
        SeekMousePos();
        HandleDirection();

        if (move) 
        {
            _Thrust(30f * Mathf.Cos(angle), 30f * Mathf.Sin(angle));
        }

        if(Input.GetAxisRaw("Fire1") != 0)
        {
            Shoot();
        }

        else
        {
            p1.Stop();
            p2.Stop();
        }
    }

    void SeekMousePos()
    {
        mousePosX = Input.mousePosition.x;
        mousePosY = Input.mousePosition.y;
    }

    void HandleDirection()
    {
        Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
        mousePosX -= objectPos.x;
        mousePosY -= objectPos.y;
        angle = Mathf.Atan2(mousePosY, mousePosX);
        spriteObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle * Mathf.Rad2Deg - 90));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemies"))
        {
            Destroy(collision.gameObject);
            TakeDamage();
        }
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
