using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Animations.Rigging;

public class RobotPlayerMovement : MonoBehaviour
{
    [SerializeField] Transform player;
    public float moveSpeed = 4.0f;
    float rotSpeed = 80.0f;
    Rigidbody rb;
    Vector3 tempPos;
    RaycastHit hit;
    public Vector3 movementVelocity;
    [SerializeField] Camera cam;
    [SerializeField] CinemachineFreeLook robotCam;
    [SerializeField] RobotShoot robotShoot;
    [SerializeField] Rig aimRig;
    [SerializeField] GameObject defaultUI;
    [SerializeField] GameObject robotUI;
    [SerializeField] Image batteryImage;


    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        batteryImage.fillAmount = ScanningVision.currBattery / ScanningVision.battery;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        movementVelocity = (transform.right * moveX + transform.forward * moveZ) * moveSpeed * Time.deltaTime;
        if (movementVelocity != Vector3.zero)
        {
            rb.velocity = movementVelocity;
        }
        //else rb.velocity = Vector3.zero;

        float targetRotation = cam.transform.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.F) || ScanningVision.currBattery <= 0)
        {
            SwitchCharacter();
        }


        //if (Physics.Raycast(transform.position + transform.up, -transform.up, out hit, 5))
        //{
        //    //
        //    Debug.DrawRay(transform.position + transform.up, -transform.up * 10, Color.red, 10f);
        //    //Debug.Log(hit.distance);
        //    tempPos = hit.point;
        //    //if (hit.distance < 1) rb.AddForce(transform.up/hit.distance/2, ForceMode.Impulse); else if(hit.distance>1)rb.AddForce(-transform.up/hit.distance, ForceMode.Impulse);
        //}


        //transform.position = Vector3.Lerp(transform.position, tempPos, moveSpeed * Time.deltaTime);
        //transform.position += tempPos - 0.3f * transform.up;

        //if (Input.GetKeyDown("space"))
        //{
        //    rb.AddForce(Vector3.up*jumpForce, ForceMode.Impulse);
        //}

    }
    private void OnEnable()
    {
        aimRig.weight = 1;
        rb.isKinematic = false;
        robotShoot.enabled = true;

    }
    private void OnDisable()
    {
        aimRig.weight = 0;
        rb.isKinematic = true;
        robotShoot.enabled = false;
    }
    void SwitchCharacter()
    {
        //player.gameObject.SetActive(true);
        player.GetComponent<PlayerLocomotion>().enabled = true;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<Ragdoll>().EnableColliders();
        robotCam.Priority = 0;
        this.enabled = false;
        GameManager.gm.isRobot = false;

        defaultUI.SetActive(true);
        robotUI.SetActive(false);

        //make player layer visible
        cam.cullingMask |= (1 << 8);
    }
}
