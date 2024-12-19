using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//WIP FOR NEW FUNCTIONALITY - NOT CURRENTLY USED IN GAMEPLAY

public class BoneCollision : MonoBehaviour
{
    [SerializeField] Rigidbody boneRigidbody; // Reference to the bone's Rigidbody
    [SerializeField] AnimationToRagdoll ragdollController; // Reference to the central ragdoll manager


    void Start()
    {

        // Get controller script
        ragdollController = GetComponentInParent<AnimationToRagdoll>();
    }

        private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Slash"))
        {
            // Calculate force based on the impact
            Vector3 hitForce = collision.relativeVelocity * 150f; // Scale force

            // Apply force to this bone's Rigidbody
            boneRigidbody.AddForceAtPosition(hitForce, collision.contacts[0].point, ForceMode.Impulse);

            // Notify the central controller to trigger ragdoll if not already done
            ragdollController.EnableRagdoll();
        }
    }

}
