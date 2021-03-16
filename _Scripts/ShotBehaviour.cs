using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehaviour : SteerableBehaviour
{
    public GameObject bullet, p1GO, p2GO;
    float angle;
    public LayerMask layerMask;
    public ParticleSystem p1, p2;
    public SpriteRenderer spriteRenderer;
    bool destroy = false;
    RaycastHit2D hit;
    SteerableBehaviour steerable;
    IDamageable Idamageable;
    public string tag;
    public float vel = 4;

    void Start() 
    {
        p1.Stop();
        p2.Stop();
        float tX, tY;
        Vector2 objectPos;

        if (tag == "_Player")
        {
            tX = Input.mousePosition.x;
            tY = Input.mousePosition.y;
            objectPos = Camera.main.WorldToScreenPoint (transform.position);
        }
        else
        {
            Transform playerTransform = GameObject.FindWithTag("_Player").transform;
            tX = playerTransform.position.x;
            tY = playerTransform.position.y;
            objectPos = transform.position;
        }
        
        tX -= objectPos.x;
        tY -= objectPos.y;
        angle = Mathf.Atan2(tY, tX);
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle * Mathf.Rad2Deg - 90));
    }

    void Update()
    {
        if (!destroy)
        {
            Thrust(vel * Mathf.Cos(angle), vel * Mathf.Sin(angle));
            hit = Physics2D.Raycast((Vector2) transform.position, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), 100f, layerMask);
            if (hit.collider.tag == tag) Destroy(gameObject);
            else if (Vector2.Distance(transform.position, hit.point) <= .4f) 
            {
                steerable = hit.transform.gameObject.GetComponent<SteerableBehaviour>();
                Idamageable = steerable as IDamageable;
                if(Idamageable != null)
                {
                    Idamageable.TakeDamage();
                }
                
                destroy = true;
                StartCoroutine(WaitToDestroy(.2f));
            }
        }
    }

    IEnumerator WaitToDestroy(float time)
    {
        spriteRenderer.enabled = false;
        p1GO.transform.position = hit.point;
        p2GO.transform.position = hit.point;
        p1.Play();
        p2.Play();
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
