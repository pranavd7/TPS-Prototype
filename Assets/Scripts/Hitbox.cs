using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Health health;

    public void OnRaycastHit(float damage, Vector3 direction)
    {
        health.TakeDamage(damage, direction);
    }
}
