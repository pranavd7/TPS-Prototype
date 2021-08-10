using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    public float blinkTimer;
    public float blinkDuration;
    public float blinkIntensity;

    Rigidbody[] rigidBodies;
    Hitbox hb;

    // Start is called before the first frame update
    void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        currentHealth = maxHealth;

        rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidBodies)
        {
            hb = rb.gameObject.AddComponent<Hitbox>();
            hb.health = this;
            if (hb.gameObject != gameObject)
            {
                hb.gameObject.layer = LayerMask.NameToLayer("Hitbox");
            }
        }

        OnStart();
    }

    void Update()
    {
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = lerp * blinkIntensity + 1.0f;
        skinnedMeshRenderer.material.color = Color.white * intensity;

        foreach (Rigidbody rb in rigidBodies)
        {
            hb = rb.gameObject.GetComponent<Hitbox>();
            if (hb.gameObject != gameObject && hb.gameObject.layer != LayerMask.NameToLayer("Hitbox"))
            {
                hb.gameObject.layer = LayerMask.NameToLayer("Hitbox");
            }
        }

        OnUpdate();
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die(direction);
        }
        else
        {
            blinkTimer = blinkDuration;
        }
        OnDamage(direction);
    }

    public void Die(Vector3 direction)
    {
        OnDeath(direction);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    protected virtual void OnStart()
    {
    }
    
    protected virtual void OnUpdate()
    {
    }

    protected virtual void OnDamage(Vector3 direction)
    {
    }

    protected virtual void OnDeath(Vector3 direction)
    {
    }

}
