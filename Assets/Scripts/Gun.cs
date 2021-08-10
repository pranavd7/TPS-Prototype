using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] TrailRenderer bulletTracer;
    [SerializeField] public Transform rayOrigin;
    [SerializeField] public string gunName;
    [SerializeField] public GameObject magazine;
    [SerializeField] public bool auto;
    [SerializeField] public float fireRate = 1;
    [SerializeField] public float cooldown = 0;
    [SerializeField] public int ammoCount;
    [SerializeField] public int clipSize;
    [SerializeField] public int totalAmmo;
    [SerializeField] public float damage;
    [SerializeField] public float forceMagnitude = 100;

    AudioSource audioSource;
    public LayerMask layerMask;
    public Ray ray;
    RaycastHit hit;


    public ActiveWeapon.weaponSlot weaponSlot;

    [HideInInspector] public WeaponRecoil recoil;

    // Start is called before the first frame update
    void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Fire(Vector3 direction)
    {
        if (ammoCount > 0)
        {
            ray.origin = rayOrigin.position;
            TrailRenderer tracer = Instantiate<TrailRenderer>(bulletTracer, ray.origin, Quaternion.identity);
            tracer.AddPosition(ray.origin);
            if (direction != null)
            {
                ray.direction = direction;
            }
            else
            {
                ray.direction = rayOrigin.forward;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                //Debug.Log(hit.point);
                //Debug.DrawLine(ray.origin, hit.point, Color.red, 1.0f);
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(ray.direction * forceMagnitude);
                }

                Hitbox hitbox = hit.collider.GetComponent<Hitbox>();
                if (hitbox)
                {
                    hitbox.OnRaycastHit(damage, ray.direction);
                }

                Instantiate<ParticleSystem>(hitEffect, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                tracer.transform.position = hit.point;
            }
            else
            {
                tracer.transform.position = rayOrigin.forward * 15;
            }
            //Instantiate<ParticleSystem>(muzzleFlash, rayOrigin.position, rayOrigin.rotation);
            muzzleFlash.Play();
            if (recoil) recoil.GenerateRecoil(gunName);
            ammoCount--;
            if (audioSource)
                audioSource.Play();
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Debug.DrawLine(ray.origin, ray.direction * 15);
    //}
}
