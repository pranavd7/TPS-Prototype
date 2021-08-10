using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public bool lockCursor;
	public float mouseSensitivity = 10;
	public Transform target;
	public float dstFromTarget = 2;
	public Vector2 pitchMinMax = new Vector2(-40, 80);

	public float rotationSmoothTime = .12f;
	Vector3 rotationSmoothVelocity;
	Vector3 currentRotation;

	float mouseX;
	float mouseY;

	void Start()
	{
		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	void LateUpdate()
	{
		mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
		mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		mouseY = Mathf.Clamp(mouseY, pitchMinMax.x, pitchMinMax.y);
		
		//followTransform.transform.rotation *= Quaternion.AngleAxis(mouseX * mouseSensitivity, Vector3.up);
        //followTransform.transform.rotation *= Quaternion.AngleAxis(mouseY * mouseSensitivity, Vector3.right);
		currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(mouseY, mouseX), ref rotationSmoothVelocity, rotationSmoothTime);
		transform.eulerAngles = currentRotation;

		transform.position = target.position - transform.forward * dstFromTarget;

	}
}
