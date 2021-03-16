using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class SteerableBehaviour : MonoBehaviour
{
    public ThrustData td;
    public Rigidbody2D rb;
    public float maxVelocity = 9f;

    private void Awake()
    {
        if (td == null)
        {
            throw new MissingReferenceException($"ShipData not set in {gameObject.name}'s Inspector");
            
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void ClampVelocity()
    {
        float y = Mathf.Clamp(rb.velocity.y, -maxVelocity, maxVelocity);
        float x = Mathf.Clamp(rb.velocity.x, -maxVelocity, maxVelocity);

        rb.velocity = new Vector2(x, y);
    }

    public virtual void _Thrust(float amountX, float amountY)
    {
        ClampVelocity();
        rb.AddForce(new Vector2(amountX, amountY));
    }

    public virtual void _ThrustForce(Vector2 force)
    {
        ClampVelocity();
        rb.AddForce(force);
    }

    public virtual void Thrust(float x, float y)
    {
        rb.MovePosition(rb.position + new Vector2(x * td.thrustIntensity.x, y * td.thrustIntensity.y) * Time.fixedDeltaTime);
    }
}
