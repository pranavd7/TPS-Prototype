using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;

public class ScanningVision : MonoBehaviour
{
    public static float battery = 100;
    public static float currBattery;

    public event EventHandler OnScanningObjectChanged;
    private const int SCANNINGSELECTED_LAYER = 14;
    int objectLayer;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera cameraOverlay1;
    [SerializeField] Camera cameraOverlay2;
    [SerializeField] CinemachineFreeLook thirdPersonCam;
    [SerializeField] CinemachineVirtualCamera firstPersonCam;
    [SerializeField] CinemachineFreeLook robotCam;
    [SerializeField] Transform player;
    [SerializeField] Transform robot;
    [SerializeField] Volume postProcessingVolume = null;
    [SerializeField] GameObject defaultUI;
    [SerializeField] GameObject robotUI;
    [SerializeField] GameObject scannerUI;
    [SerializeField] Image batteryImage;
    [SerializeField] UniversalAdditionalCameraData additionalCameraData;
    [SerializeField] LayerMask layerMask = default(LayerMask);

    public Vector3 offset;

    ActiveWeapon aw;
    PlayerLocomotion pl;
    Ragdoll rd;

    private bool isActive;
    private GameObject lastActiveScannedGameObject;

    private void Start()
    {
        aw = player.GetComponent<ActiveWeapon>();
        pl = player.GetComponent<PlayerLocomotion>();
        rd = player.GetComponent<Ragdoll>();

        currBattery = battery/2;

        SetIsActive(false);
    }

    private void Update()
    {
        if (GameManager.gm.isRobot || !GameManager.gm.isAlive)
        {
            currBattery -= Time.deltaTime * 5;
            return;
        }
        else if (currBattery < battery)
            currBattery += Time.deltaTime * 3;

        batteryImage.fillAmount = currBattery / battery;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetIsActive(!isActive);
        }

        if (lastActiveScannedGameObject != null)
        {
            SetAllChildrenToLayer(lastActiveScannedGameObject, objectLayer);
            lastActiveScannedGameObject = null;
            OnScanningObjectChanged?.Invoke(lastActiveScannedGameObject, EventArgs.Empty);
        }

        if (isActive)
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit raycastHit, 100f, layerMask))
            {
                if (raycastHit.collider.gameObject.TryGetComponent<ScannableObject>(out ScannableObject scannableObject))
                {
                    // This object can be scanned
                    SetAllChildrenToLayer(raycastHit.collider.gameObject, SCANNINGSELECTED_LAYER);

                    lastActiveScannedGameObject = raycastHit.collider.gameObject;
                    objectLayer = scannableObject.layer;
                    OnScanningObjectChanged?.Invoke(lastActiveScannedGameObject, EventArgs.Empty);

                    if (scannableObject.hackable == "yes" && Input.GetKeyDown(KeyCode.F) && currBattery >= battery)
                    {
                        SwitchPlayer();
                        OnScanningObjectChanged?.Invoke(null, EventArgs.Empty);
                    }
                }
            }
        }
    }

    private void SwitchPlayer()
    {
        aw.Holster();
        SetIsActive(false);
        robot.GetComponent<RobotPlayerMovement>().enabled = true;
        robotCam.Priority = 3;
        pl.StopMotion();
        pl.enabled = false;
        rd.DisableColliders();

        player.GetComponent<CharacterController>().enabled = false;
        GameManager.gm.isRobot = true;
        defaultUI.SetActive(false);
        robotUI.SetActive(true);

        //make player layer invisible
        mainCamera.cullingMask &= ~(1 << 8);
    }

    public static void SetAllChildrenToLayer(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform)
        {
            SetAllChildrenToLayer(child.gameObject, layer);
        }
    }

    private void SetIsActive(bool isActive)
    {
        this.isActive = isActive;

        if (isActive)
        {
            aw.enabled = false;
            pl.scanning = true;
            ShowFirstPersonView();
            cameraOverlay1.gameObject.SetActive(true);
            cameraOverlay2.gameObject.SetActive(true);
            postProcessingVolume.gameObject.SetActive(true);
            defaultUI.SetActive(false);
            scannerUI.SetActive(true);
            additionalCameraData.SetRenderer(1);
        }
        else
        {
            ShowOverheadView();
            aw.enabled = true;
            pl.scanning = false;
            cameraOverlay1.gameObject.SetActive(false);
            cameraOverlay2.gameObject.SetActive(false);
            postProcessingVolume.gameObject.SetActive(false);
            defaultUI.SetActive(true);
            scannerUI.SetActive(false);
            additionalCameraData.SetRenderer(0);
        }
    }

    public void ShowOverheadView()
    {
        firstPersonCam.Priority = 0;
        thirdPersonCam.Priority = 1;
        //make player layer visible
        mainCamera.cullingMask |= (1 << 8);
    }

    public void ShowFirstPersonView()
    {
        firstPersonCam.Priority = 1;
        thirdPersonCam.Priority = 0;
        //make player layer invisible
        mainCamera.cullingMask &= ~(1 << 8);
    }
}