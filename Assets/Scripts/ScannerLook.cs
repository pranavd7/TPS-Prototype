using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerLook : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    private CharacterController characterController;

    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] float moveSpeed = 2f;
    private float cameraVerticalAngle;
    private float characterVelocityY;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCharacterLook();
        HandleCharacterMovement();
    }

    private void HandleCharacterLook()
    {
        float lookX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float lookY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate the transform with the input speed around its local Y axis
        transform.Rotate(Vector3.up * lookX);

        // Add vertical inputs to the camera's vertical angle
        cameraVerticalAngle -= lookY;

        // Limit the camera's vertical angle to min/max
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -85f, 88f);

        // Apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
        playerCamera.transform.localRotation = Quaternion.Euler(cameraVerticalAngle, 0, 0);
    }

    private void HandleCharacterMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 characterVelocity = (transform.right * moveX + transform.forward * moveZ) * moveSpeed * Time.deltaTime;

        // Apply gravity to the velocity
        float gravityDownForce = -60f;
        characterVelocityY += gravityDownForce * Time.deltaTime;


        // Apply Y velocity to move vector
        characterVelocity.y = characterVelocityY;

        // Move Character Controller
        characterController.Move(characterVelocity);
    }
}
