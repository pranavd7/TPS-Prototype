using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;

public class ActiveWeapon : MonoBehaviour
{
    Gun[] weapons = new Gun[2];
    int activeWeaponIndex;

    public enum weaponSlot
    {
        Primary = 0, Secondary = 1
    }
    [SerializeField] Camera cam;
    [SerializeField] Transform[] weaponParents;
    [SerializeField] Transform leftHandGrip;
    [SerializeField] Transform rightHandGrip;
    [SerializeField] public Animator rigController;
    [SerializeField] Rig aimRig;
    [SerializeField] Rig bodyAimRig;
    [SerializeField] CinemachineFreeLook playerCam;
    CinemachineCameraOffset cameraOffset;
    [SerializeField] AmmoHUD hud;

    float aimDuration = 0.1f;
    public float turnSpeed = 15;
    public bool aiming = false;
    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    bool holstered;
    public bool isReloading;
    public bool isSprinting;
    float nextTimeToFire = 0;
    Gun gun;
    RaycastHit hit;
    Vector3 shootDirection;

    // Start is called before the first frame update
    void Start()
    {
        //cam = Camera.main;
        Gun[] existingWeapon = GetComponentsInChildren<Gun>();
        int currentWeapon = 0;
        foreach (Gun gun in existingWeapon)
        {
            weapons[(int)gun.weaponSlot] = gun;
            EquipWeapon(weapons[currentWeapon]);
        }
        holstered = rigController.GetBool("holster_weapon");
        cameraOffset = playerCam.GetComponent<CinemachineCameraOffset>();

        hud.Refresh(0, 0, false);
    }

    private void Update()
    {
        if (GameManager.gm.isRobot || !GameManager.gm.isAlive || GameManager.gm.paused)
            return;

        if (GameManager.gm.wave1finished)
        {
            StartCoroutine(HolsterWeapon(activeWeaponIndex));
            return;
        }

        gun = GetWeapon(activeWeaponIndex);
        if (gun)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (holstered)
                    StartCoroutine(ActivateWeapon(activeWeaponIndex));
                else
                    StartCoroutine(HolsterWeapon(activeWeaponIndex));
            }

            //Debug.Log(weapons[0].gunName);
            //Debug.Log(weapons[1].gunName);

            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                StartCoroutine(SwitchWeapon(activeWeaponIndex, (activeWeaponIndex + 1) % weapons.Length));
                //activeWeaponIndex /= weapons.Length;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartCoroutine(SwitchWeapon(activeWeaponIndex, 0));
            if (Input.GetKeyDown(KeyCode.Alpha2))
                StartCoroutine(SwitchWeapon(activeWeaponIndex, 1));

            if (gun && !holstered && Input.GetMouseButton(1))
            {
                //    aimRig.weight += Time.deltaTime / aimDuration;
                //    bodyAimRig.weight += Time.deltaTime / aimDuration;

                cameraOffset.m_Offset.z += Time.deltaTime / aimDuration;
                cameraOffset.m_Offset.z = Mathf.Min(cameraOffset.m_Offset.z, 1);
                playerCam.m_Lens.FieldOfView = 30;
                aiming = true;
                gun.recoil.recoilMultiplier = 0.5f;
            }
            else
            {
                //aimRig.weight -= Time.deltaTime / aimDuration;
                //bodyAimRig.weight -= Time.deltaTime / aimDuration;
                cameraOffset.m_Offset.z -= Time.deltaTime / aimDuration;
                cameraOffset.m_Offset.z = Mathf.Max(cameraOffset.m_Offset.z, 0);
                playerCam.m_Lens.FieldOfView = 40;
                aiming = false;
                gun.recoil.recoilMultiplier = 1;
            }

            isSprinting = rigController.GetBool("isSprinting");
            if (!holstered && !isReloading && !isSprinting)
            {
                if (gun.auto)
                {
                    if (Input.GetMouseButton(0) && Time.time > nextTimeToFire)
                    {
                        nextTimeToFire = Time.time + 1 / gun.fireRate;

                        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
                        {
                            shootDirection = hit.point - gun.rayOrigin.position;
                        }
                        else
                        {
                            shootDirection = gun.rayOrigin.forward;
                        }

                        gun.Fire(shootDirection);
                        hud.Refresh(gun.ammoCount, gun.totalAmmo, true);
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        gun.recoil.ResetRecoil();
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire)
                    {
                        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
                        {
                            shootDirection = hit.point - gun.rayOrigin.position;
                        }
                        else
                        {
                            shootDirection = gun.rayOrigin.forward;
                        }

                        gun.Fire(shootDirection);
                        nextTimeToFire = Time.time + gun.cooldown;
                        hud.Refresh(gun.ammoCount, gun.totalAmmo, true);
                    }
                }
            }
        }
        else
        {
            hud.Refresh(0, 0, false);
        }
    }

    //private void FixedUpdate()
    //{
    //    gun = GetWeapon(activeWeaponIndex);
    //    if (gun)
    //    {
    //        if (!holstered)
    //        {
    //            float targetRotation = Camera.main.transform.eulerAngles.y;
    //            //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
    //            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), turnSpeed * Time.fixedDeltaTime);
    //        }
    //    }
    //}

    public Gun GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }
    Gun GetWeapon(int index)
    {
        if (index < 0 || index > weapons.Length - 1)
            return null;
        else
            return weapons[index];
    }

    public void EquipWeapon(Gun gun)
    {
        ScanningVision.SetAllChildrenToLayer(gun.gameObject, 8);

        int gunIndex = (int)gun.weaponSlot;
        if (weapons[gunIndex])
        {
            Destroy(weapons[gunIndex].gameObject);
        }
        weapons[gunIndex] = gun;
        weapons[gunIndex].transform.parent = weaponParents[gunIndex];
        weapons[gunIndex].transform.localPosition = Vector3.zero;
        weapons[gunIndex].transform.localRotation = Quaternion.identity;
        weapons[gunIndex].recoil.playerCam = playerCam;
        weapons[gunIndex].recoil.rigController = rigController;

        StartCoroutine(SwitchWeapon(activeWeaponIndex, gunIndex));

    }


    IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        if (holsterIndex == activateIndex)
            holsterIndex = -1;
        yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex;
    }

    public IEnumerator HolsterWeapon(int index)
    {
        holstered = true;
        Gun weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.SetBool("holster_weapon", true);
            do
            {
                yield return new WaitForEndOfFrame();
            } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        }
        hud.Refresh(0, 0, false);

    }
    IEnumerator ActivateWeapon(int index)
    {
        Gun weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.SetBool("holster_weapon", false);
            rigController.Play("equip_" + weapon.gunName);
            do
            {
                yield return new WaitForEndOfFrame();
            } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        }
        holstered = false;
        hud.Refresh(weapons[index].ammoCount, weapons[index].totalAmmo, true);
    }
    public void DropWeapon()
    {
        foreach (Gun gun in weapons)
        {
            if (gun)
            {
                gun.GetComponent<BoxCollider>().enabled = true;
                gun.gameObject.AddComponent<Rigidbody>();
                gun.transform.parent = null;
                //weapons[activeWeaponIndex] = null;
            }
        }
    }

    public void Holster()
    {
        if (gun && !holstered)
            StartCoroutine(HolsterWeapon(activeWeaponIndex));
    }
}
