using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToRagdoll : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Collider myCollider;
    [SerializeField] Rigidbody[] rigidBodies;

    // Start is called before the first frame update
    void Start()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidBodies)
        {
            rb.isKinematic = true; // Initially disable physics
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slash"))
        {
            //Vector3 hitForce = other.attachedRigidbody.velocity * 10f; // Scale force
            Debug.Log($"Triggered by: {other.name}, Rigidbody attached: {other.attachedRigidbody != null}");

            Vector3 hitForce = other.attachedRigidbody.velocity * 100f; // Scale force
            EnableRagdoll();
            ApplyForce(hitForce, other.transform.position);
        }
    }


    void EnableRagdoll()
    {
        animator.enabled = false;
        foreach (var rb in rigidBodies)
        {
            rb.isKinematic = false; // Enable physics
        }
    }


    void ApplyForce(Vector3 force, Vector3 hitPoint)
    {
        Rigidbody closestRigidBody = null;
        float closestDistance = float.MaxValue;

        foreach (var rb in rigidBodies)
        {
            float distance = Vector3.Distance(rb.worldCenterOfMass, hitPoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestRigidBody = rb;
            }
        }

        if (closestRigidBody != null)
        {
            closestRigidBody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
        }
    }

}
