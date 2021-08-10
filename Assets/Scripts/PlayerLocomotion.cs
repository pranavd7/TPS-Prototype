using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class PlayerLocomotion : MonoBehaviour
{
    Animator animator;
    public Animator rigController;
    Rigidbody rb;
    ActiveWeapon activeWeapon;

    //public Cinemachine.AxisState xAxis;
    //public Cinemachine.AxisState yAxis;
    //public Transform cameraLookAt;

    Vector3 gravityVelocity;
    Vector3 midairVelocity;
    public float moveSpeed;
    public float turnSpeed = 15;
    public float jumpHeight;
    float jumpVelocity;
    public float stepDown;
    public float gravity;
    public float fallingThreshold;
    bool isJumping;
    bool isFalling;
    bool isRunJumping;
    bool isGrounded;
    bool isCrouched;
    public bool scanning;

    public float MaxWalkingSpeed = .5f;
    public float mouseSensitivity = 10;

    [SerializeField] CharacterController cc;
    [SerializeField] Transform cameraT;

    Vector3 groundNormal;
    float groundCheckDistance = 0.15f;

    float h;
    float v;
    bool runPressed;
    bool crouchPressed;
    float mouseX;
    float mouseY;
    Vector3 moveVelocity;
    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;
    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    Vector3 camDir;
    Gun gun;

    [SerializeField] Rig bodyAim;

    float animX;
    float animZ;
    float gravitySmall = 1;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        activeWeapon = GetComponentInChildren<ActiveWeapon>();
        //cameraT = Camera.main.transform;

        jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();

        if (Vector3.Angle(transform.forward, Vector3.ProjectOnPlane(cameraT.forward, Vector3.up).normalized) > 80)
        {
            bodyAim.weight -= mouseSensitivity * Time.deltaTime;
        }
        else
        {
            bodyAim.weight += mouseSensitivity * Time.deltaTime;
        }

        if (GameManager.gm.wave1finished)
            return;

        moveVelocity = new Vector3(h, 0, v);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!isCrouched)
                isCrouched = true;
            else
                isCrouched = false;
            animator.SetBool("crouched", isCrouched);
        }

        if (isCrouched)
        {
            animator.SetFloat("Velocity XZ", moveVelocity.normalized.magnitude, speedSmoothTime, Time.deltaTime);
        }
        else
        {
            if (runPressed && moveVelocity != Vector3.zero && !activeWeapon.aiming)
            {
                if (gun)
                {
                    rigController.SetBool("isSprinting", true);
                }
            }
            else
            {
                rigController.SetBool("isSprinting", false);
            }
            animX = runPressed && !scanning && !activeWeapon.aiming ? 2 : 1;
            animZ = runPressed && !scanning && !activeWeapon.aiming ? 2 : 1;
            animator.SetFloat("Velocity X", animX * h, speedSmoothTime, Time.deltaTime);
            animator.SetFloat("Velocity Z", animZ * v, speedSmoothTime, Time.deltaTime);
            animator.SetFloat("Velocity XZ", moveVelocity.normalized.magnitude, speedSmoothTime, Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !scanning)
        {
            Jump();
        }

    }

    private void FixedUpdate()
    {
        if (GameManager.gm.wave1finished)
        {
            StopMotion();
            return;
        }

        gun = activeWeapon.GetActiveWeapon();

        moveVelocity = new Vector3(h, 0, v);
        float targetRotation = cameraT.eulerAngles.y;
        if (moveVelocity != Vector3.zero || gun)
        {
            //if (isCrouched)
            //{
            //    if (gun)
            //    {
            //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), turnSpeed * Time.fixedDeltaTime);
            //    }
            //    else
            //    {
            //        float targetAngle = Mathf.Atan2(moveVelocity.x, moveVelocity.z) * Mathf.Rad2Deg + targetRotation;
            //        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //        transform.rotation = Quaternion.Euler(0, angle, 0);
            //    }
            //    //transform.forward = moveVelocity;
            //}

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), turnSpeed * Time.fixedDeltaTime);
            //transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);

        }

        gravityVelocity.y -= gravitySmall * Time.fixedDeltaTime;  //gravitySmall=1
        cc.Move(gravityVelocity * Time.fixedDeltaTime);
        isGrounded = cc.isGrounded;
        //Debug.Log(isGrounded);

        //if (isJumping)
        //{
        //    velocity.y -= gravity * Time.fixedDeltaTime;
        //    cc.Move(velocity * Time.fixedDeltaTime);
        //    isJumping = !cc.isGrounded;
        //    //animator.SetBool("jumping", isJumping);
        //}
        //else
        //{
        //    cc.Move(Vector3.down * stepDown);
        //    if (!cc.isGrounded)
        //    {
        //        isJumping = true;
        //        //animator.SetBool("jumping", true);
        //    }
        //}

        if (cc.isGrounded)
        {
            gravityVelocity.x = 0;
            gravityVelocity.z = 0;
            gravityVelocity.y -= gravity * Time.fixedDeltaTime;

            cc.Move(gravityVelocity * Time.fixedDeltaTime);
            if (gravityVelocity.y < 0)
                gravityVelocity.y = 0;
            //Debug.Log(gravityVelocity.y);
            if (isJumping && !isRunJumping)
            {
                animator.SetTrigger("Land");
                isFalling = false;
            }
            isRunJumping = false;
            isJumping = false;
            //animator.SetBool("jumping", isJumping);
        }
        else
        {
            if (isJumping)
            {
                midairVelocity = (moveVelocity.x * transform.right + moveVelocity.z * transform.forward) * moveSpeed;
                //velocity.y = tempYvelocity;
                gravityVelocity.y -= gravity * Time.fixedDeltaTime;
                //tempYvelocity = velocity.y;
                cc.Move(gravityVelocity * Time.fixedDeltaTime);
                if (gravityVelocity.y < fallingThreshold && !isFalling)
                {
                    animator.SetTrigger("Falling");
                    isFalling = true;
                    isRunJumping = false;
                }
            }
            else
            {   //move down quickly a bit for checking for stairs
                cc.Move(Vector3.down * stepDown); //* Time.fixedDeltaTime);

                //if still not grounded meaning player still in air, set isJumping to true to fall with required speed
                if (!cc.isGrounded)
                {
                    isJumping = true;
                    animator.SetTrigger("Falling");
                }
            }
        }

        if (isCrouched)
        {

        }
        //cc.Move(velocity*Time.fixedDeltaTime);
    }

    void GetInput()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        crouchPressed = Input.GetKey(KeyCode.LeftControl);
        runPressed = Input.GetKey(KeyCode.LeftShift);

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

    }

    void Jump()
    {
        moveVelocity = new Vector3(h, 0, v);
        if (isJumping)
        {
            return;
        }
        if (moveVelocity != Vector3.zero && runPressed)
        {
            animator.SetTrigger("RunningJump");
            isRunJumping = true;
            //isJumping = true;
        }
        else
        {
            animator.SetTrigger("Jump");
            //isJumping = true;
        }

    }
    void ApplyJumpForce()
    {
        jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
        gravityVelocity.y = jumpVelocity;
        //if (animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Jumping Layer")).IsName("Running Jump"))
        //    isRunJumping = true;
        isJumping = true;
    }

    void EnableRunJump()
    {
        isRunJumping = true;
    }

    void CheckIfGrounded()
    {
        RaycastHit hitInfo;

        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));

        // 0.1f is a small offset to start the ray from inside the character
        //  transform position of character is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
        {
            groundNormal = hitInfo.normal;
            //animator.applyRootMotion = true;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            //animator.applyRootMotion = false;
            groundNormal = Vector3.up;
        }
    }


    public void StopMotion()
    {
        h = 0;
        v = 0;
        animator.SetFloat("Velocity X", 0);
        animator.SetFloat("Velocity Z", 0);
        cc.detectCollisions = false;
        //animator.applyRootMotion = false;
    }

    public void EnableMotion()
    {
        //animator.applyRootMotion = true;
        cc.detectCollisions = true;
    }


}
