using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiWeapon : MonoBehaviour
{
    Transform player;
    AiAgent agent;
    public Gun gun;
    RaycastHit hit;
    GameObject magazine;
    [SerializeField] Transform handLeft;
    [SerializeField] WeaponAnimationEvent animationEvent;
    [SerializeField] Animator rigController;

    bool reloading;
    float nextTimeToFire = 0.0f;
    public float inaccuracy = 0.0f;

    Vector3 shootDirection;
    void Start()
    {
        gun = GetComponentInChildren<Gun>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = FindObjectOfType<AiAgent>();
        animationEvent.weaponAnimationEvent.AddListener(OnAnimationEvent);
    }

    // Update is called once per frame
    public void ShootUpdate()
    {
        shootDirection = agent.target.position - gun.rayOrigin.position;
        shootDirection += Random.insideUnitSphere * inaccuracy;
        if (!reloading)
        {
            if (gun.auto)
            {
                if (Time.time > nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1 / gun.fireRate;

                    gun.Fire(shootDirection);
                }
            }
            else
            {
                if (Time.time > nextTimeToFire)
                {
                    gun.Fire(shootDirection);
                    nextTimeToFire = Time.time + gun.cooldown;
                }
            }

            if (gun.ammoCount <= 0 && gun.totalAmmo > 0)
            {
                rigController.SetTrigger("reload");
                reloading = true;
            }
        }
    }

    public void DropWeapon()
    {
        gun.GetComponent<BoxCollider>().enabled = true;
        ScanningVision.SetAllChildrenToLayer(gun.gameObject, 9);
        gun.gameObject.AddComponent<Rigidbody>();
        gun.transform.parent = null;
    }

    void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "detach_mag":
                DetachMagazine();
                break;
            case "drop_mag":
                DropMagazine();
                break;
            case "refill_mag":
                RefillMagazine();
                break;
            case "attach_mag":
                AttachMagazine();
                break;
            case "enable_shoot":
                EnableShooting();
                break;
        }
    }

    void DetachMagazine()
    {
        magazine = Instantiate(gun.magazine, handLeft, true);
        magazine.transform.localScale = gun.magazine.transform.localScale;
        gun.magazine.SetActive(false);
    }

    void DropMagazine()
    {
        GameObject droppedMag = Instantiate(magazine, magazine.transform.position, magazine.transform.rotation);
        droppedMag.transform.localScale = magazine.transform.localScale;
        droppedMag.AddComponent<Rigidbody>();
        //droppedMag.AddComponent<BoxCollider>();
        magazine.SetActive(false);
    }

    void RefillMagazine()
    {
        magazine.SetActive(true);
    }

    void AttachMagazine()
    {
        gun.magazine.SetActive(true);
        Destroy(magazine);
        int ammoUsed = gun.clipSize - gun.ammoCount;
        if (gun.totalAmmo > ammoUsed)
        {
            gun.ammoCount += ammoUsed;
            gun.totalAmmo -= ammoUsed;
        }
        else
        {
            gun.ammoCount += gun.totalAmmo;
            gun.totalAmmo = 0;
        }
        rigController.ResetTrigger("reload");
    }

    void EnableShooting()
    {
        reloading = false;
    }
}
