using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReload : MonoBehaviour
{
    [SerializeField] Animator rigController;
    [SerializeField] WeaponAnimationEvent animationEvent;
    [SerializeField] Transform handLeft;
    [SerializeField] AmmoHUD hud;
    ActiveWeapon activeWeapon;
    GameObject magazine;

    bool isSprinting;

    // Start is called before the first frame update
    void Start()
    {
        animationEvent.weaponAnimationEvent.AddListener(OnAnimationEvent);
        activeWeapon = GetComponent<ActiveWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        isSprinting = activeWeapon.rigController.GetBool("isSprinting");
        Gun gun = activeWeapon.GetActiveWeapon();
        if (gun)
        {
            if ((Input.GetKeyDown(KeyCode.R) || gun.ammoCount <= 0) && (gun.totalAmmo > 0 && gun.ammoCount != gun.clipSize) && !isSprinting)
            {
                rigController.SetTrigger("reload");
                activeWeapon.isReloading = true;
            }
        }
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
        }
    }

    void DetachMagazine()
    {
        Gun gun = activeWeapon.GetActiveWeapon();
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
        Gun gun = activeWeapon.GetActiveWeapon();
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
        activeWeapon.isReloading = false;
        hud.Refresh(gun.ammoCount, gun.totalAmmo, true);
    }
}
