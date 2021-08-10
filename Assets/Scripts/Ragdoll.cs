using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidBodies;
    Collider[] colliders;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();
        DeactivateRagdoll();
    }

    public void DeactivateRagdoll()
    {
        foreach (Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = true;
        }
        animator.enabled = true;
    }
    
    public void ActivateRagdoll()
    {
        foreach (Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = false;
        }
        animator.enabled = false;
    }

    public void DisableColliders()
    {
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    public void EnableColliders()
    {
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }
    }

    public void ApplyForce(Vector3 force)
    {
        var rb = animator.GetBoneTransform(HumanBodyBones.Spine).GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.VelocityChange);
    }
}
