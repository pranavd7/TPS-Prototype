using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] Gun gunPrefab;
    [SerializeField] GameObject pickupUI;
    Animator animator;

    private void Start()
    {
        //pickupUI.SetActive(false);
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            animator.SetBool("activate", true);
    }

    private void OnTriggerStay(Collider other)
    {
        //pickupUI.SetActive(true);
        if (Input.GetKeyDown(KeyCode.E))
        {
            ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();
            if (activeWeapon)
            {
                Gun gun = Instantiate(gunPrefab);
                activeWeapon.EquipWeapon(gun);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //pickupUI.SetActive(false);
        animator.SetBool("activate", false);
    }
}