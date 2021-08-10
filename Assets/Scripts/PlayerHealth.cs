using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : Health
{
    Ragdoll ragdoll;
    ActiveWeapon activeWeapon;
    PlayerLocomotion playerMovement;
    VolumeProfile postProcessing;
    Vignette vignette1;
    public Image bloodyOverlay;

    float percent;
    float time = 0.5f;
    [SerializeField] float waitSecForHeal = 2;
    [SerializeField] float healSpeed;
    Color tempColor;

    protected override void OnStart()
    {
        ragdoll = GetComponent<Ragdoll>();
        activeWeapon = GetComponent<ActiveWeapon>();
        playerMovement = GetComponent<PlayerLocomotion>();
        postProcessing = FindObjectOfType<Volume>().profile;
        //bloodyOverlay.enabled = false;
        tempColor = bloodyOverlay.color;
    }

    protected override void OnUpdate()
    {
        time += Time.deltaTime;
        if (!IsDead() && time > waitSecForHeal && currentHealth < maxHealth)
        {
            currentHealth += healSpeed * Time.deltaTime;
        }

        if (postProcessing.TryGet(out vignette1))
        {
            percent = 1.0f - currentHealth / maxHealth;
            vignette1.intensity.value = percent * 0.6f;
        }

        percent = 1.0f - (time / 0.5f);

        tempColor.a = percent;
        bloodyOverlay.color = tempColor;
    }

    protected override void OnDamage(Vector3 direction)
    {
        time = 0;
    }

    protected override void OnDeath(Vector3 direction)
    {
        GameManager.gm.isAlive = false;
        ragdoll.ActivateRagdoll();
        activeWeapon.DropWeapon();
        playerMovement.enabled = false;
        activeWeapon.enabled = false;
        //bloodyOverlay.enabled = true;
    }
}
