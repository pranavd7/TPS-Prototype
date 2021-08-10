using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLegTargetBiped : MonoBehaviour
{
    //current positions and rotations
    Vector3 currentPosition;
    Quaternion currentRotation;
    Vector3 oldPosition;
    Quaternion oldRotation;
    RobotPlayerMovement player;

    //offset transform
    [SerializeField] Vector3 offsetPos;
    [SerializeField] Quaternion offsetRot;

    //assignable objects
    [SerializeField] Transform body;
    [SerializeField] Transform desiredPosition;
    [SerializeField] IKLegTargetBiped otherLeg;
    [SerializeField] AnimationCurve stepDistanceCurve;
    [SerializeField] AnimationCurve offsetDistanceCurve;

    //variables
    float moveSpeed = 50;
    float turnSpeed = 360;
    public float currentStepDuration = 0.5f;
    float time;
    float offsetStepDistance;
    float stepHeight = 0.5f;
    public bool grounded = true;
    public bool movable = false;

    float lerp;
    Vector3 dirToTarget;

    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<RobotPlayerMovement>();
        time = currentStepDuration;
        oldPosition = currentPosition = transform.position;
        currentRotation = transform.rotation;
        oldRotation = transform.localRotation;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentPosition, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, currentRotation, turnSpeed * Time.deltaTime);


        //Cast a ray and if we hit a collider, set the desiredYPosition to the hit Y point.
        if (Physics.Raycast(desiredPosition.position + Vector3.up, -Vector3.up, out hit, 2))
        {
            //Debug.DrawRay(new Vector3(desiredPosition.position.x, desiredPosition.position.y + 1, desiredPosition.position.z), -Vector3.up, Color.red, 5f);
            desiredPosition.position = hit.point;
            desiredPosition.transform.rotation = Quaternion.LookRotation(Vector3.Cross(desiredPosition.transform.right, hit.normal), hit.normal);
        }

        //dirToTarget = body.InverseTransformPoint(desiredPosition.position) - body.InverseTransformPoint(oldPosition);
        dirToTarget = (desiredPosition.position ) - (oldPosition);
        //Debug.DrawRay(oldPosition, dirToTarget);
        //Debug.Log("old"+ body.InverseTransformPoint(oldPosition) +"des"+ body.InverseTransformPoint(desiredPosition.position));
        float dirToTargetDot = Vector3.Dot(dirToTarget.normalized, body.forward);
        float stepDistance = stepDistanceCurve.Evaluate(dirToTargetDot);
        //Debug.Log(dirToTarget.magnitude + "," + dirToTargetDot + ',' + stepDistance);

        float angleToTarget = Vector3.Angle(desiredPosition.forward, transform.forward);

        if ((dirToTarget.magnitude > stepDistance) && grounded && movable)
        {
            offsetStepDistance = offsetDistanceCurve.Evaluate(dirToTargetDot);
            time = 0;
            StartCoroutine(TakeStepDist());
        }
        else if (angleToTarget > 45 && grounded && movable)
        {
            time = 0;
            StartCoroutine(TakeStepRot());
        }
        else
        {
            if (Physics.Raycast(currentPosition + Vector3.up, -Vector3.up, out hit, Mathf.Infinity))
            {
                //Debug.DrawRay(new Vector3(desiredPosition.position.x, desiredPosition.position.y + 1, desiredPosition.position.z), -Vector3.up, Color.red, 5f);
                currentPosition = hit.point + offsetPos;
                //currentPosition.y += offsetPos.y;
            }
            //grounded = true;
            oldPosition = currentPosition;
        }
        //Debug.Log(time);
    }

    IEnumerator TakeStepDist()
    {
        grounded = false;
        lerp = time / currentStepDuration;
        while (lerp < 1)
        {
            lerp = time / currentStepDuration;
            currentPosition = Vector3.Lerp(oldPosition, desiredPosition.position + dirToTarget.normalized * offsetStepDistance, lerp);
            currentPosition += offsetPos;
            currentPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            currentRotation = Quaternion.Slerp(transform.rotation, desiredPosition.rotation, lerp);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        grounded = true;
    }
    IEnumerator TakeStepRot()
    {
        grounded = false;
        lerp = time / currentStepDuration;
        while (lerp < 1)
        {
            lerp = time / currentStepDuration;
            currentPosition = Vector3.Lerp(oldPosition, desiredPosition.position, lerp);
            currentPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            currentRotation = Quaternion.Slerp(transform.rotation, desiredPosition.rotation, lerp);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        grounded = true;
    }

    public void OffsetTarget(int i)
    {
        transform.position += i % 2 == 0 ? 0.15f * body.forward : -0.15f * body.forward;
    }
}