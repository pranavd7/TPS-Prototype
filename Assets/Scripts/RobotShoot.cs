using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RobotShoot : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] CinemachineFreeLook robotCam;
    [SerializeField] ParticleSystem[] muzzleFlash;
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] TrailRenderer bulletTracer;
    [SerializeField] Transform[] rayOrigin;
    [SerializeField] public float forceMagnitude = 100;
    [SerializeField] int fireRate;
    [SerializeField] float damage;
    public LayerMask layerMask;

    Animator animator;
    public Ray ray;
    RaycastHit hit;
    float nextTimeToFire = 0;
    Vector3[] shootDirection;
    [SerializeField] Vector2[] recoil;

    int shootBig = Animator.StringToHash("ShootBigCanons");
    int shootSmall = Animator.StringToHash("ShootSmallCanons");
    TrailRenderer tracer1;
    TrailRenderer tracer2;
    AudioSource audioSource;
    CinemachineImpulseSource camShake;

    //bool to alternate fire between primary canons
    bool bigFire = true;
    int index;
    float vRecoil;
    float hRecoil;
    public float recoilMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        shootDirection = new Vector3[rayOrigin.Length];
        audioSource = GetComponent<AudioSource>();
        camShake = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.gm.isAlive || GameManager.gm.paused || GameManager.gm.wave1finished)
            return;

        if (Input.GetMouseButton(0) && Time.time > nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1 / fireRate;

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask))
            {
                if (bigFire)
                {
                    shootDirection[0] = hit.point - rayOrigin[0].position;
                    shootDirection[1] = hit.point - rayOrigin[1].position;
                }
                else
                {
                    shootDirection[0] = hit.point - rayOrigin[0].position;
                    shootDirection[1] = hit.point - rayOrigin[1].position;
                }
            }
            else
            {
                shootDirection[0] = rayOrigin[0].forward;
                shootDirection[1] = rayOrigin[1].forward;
                shootDirection[2] = rayOrigin[2].forward;
                shootDirection[3] = rayOrigin[3].forward;
            }

            Fire();
        }
    }

    void Fire()
    {
        if (bigFire)
        {
            animator.Play(shootBig);
            tracer1 = Instantiate<TrailRenderer>(bulletTracer, rayOrigin[0].position, Quaternion.identity);
            tracer2 = Instantiate<TrailRenderer>(bulletTracer, rayOrigin[1].position, Quaternion.identity);
            tracer1.AddPosition(rayOrigin[0].position);
            tracer2.AddPosition(rayOrigin[1].position);
            muzzleFlash[0].Play();
            muzzleFlash[1].Play();
        }
        else
        {
            animator.Play(shootSmall);
            tracer1 = Instantiate<TrailRenderer>(bulletTracer, rayOrigin[2].position, Quaternion.identity);
            tracer2 = Instantiate<TrailRenderer>(bulletTracer, rayOrigin[3].position, Quaternion.identity);
            tracer1.AddPosition(rayOrigin[2].position);
            tracer2.AddPosition(rayOrigin[3].position);
            muzzleFlash[2].Play();
            muzzleFlash[3].Play();
        }

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask))
        {
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
            tracer1.transform.position = hit.point;
            tracer2.transform.position = hit.point;
        }
        else
        {
            tracer1.transform.position = cam.transform.forward * 15;
            tracer2.transform.position = cam.transform.forward * 15;
        }
        bigFire = !bigFire;
        if (audioSource)
            audioSource.Play();
        GenerateRecoil();
    }

    public void GenerateRecoil()
    {
        //time = duration;
        hRecoil = recoil[index].x;
        vRecoil = recoil[index].y;

        robotCam.m_YAxis.Value -= vRecoil * recoilMultiplier;
        robotCam.m_XAxis.Value -= hRecoil * recoilMultiplier;
        camShake.GenerateImpulse(cam.transform.forward);

        index = (index + 1) % recoil.Length;
    }
}
