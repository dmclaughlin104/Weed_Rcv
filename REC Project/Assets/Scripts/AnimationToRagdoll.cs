using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationToRagdoll : MonoBehaviour
{
    [SerializeField] Animator enemyAnim;
    [SerializeField] Collider myCollider;
    [SerializeField] Rigidbody[] rigidBodies;

    private float velocityThreshold = 5f; // Threshold to switch between animation and ragdoll death state
    private float velocityScaleForce = 1f; //no multiplying force required currently

    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();
    private Transform[] bones;

    private SwordVelocityManager velocityManager;
    private GunController gunControllerScript;

    void Start()
    {
        // Get all rigid bodies
        rigidBodies = GetComponentsInChildren<Rigidbody>();

        // Get controller script
        gunControllerScript = GameObject.Find("Gun Controller").GetComponent<GunController>();

        // Get all bones
        bones = GetComponentsInChildren<Transform>();
        foreach (var bone in bones)
        {
            originalPositions[bone] = bone.localPosition;
            originalRotations[bone] = bone.localRotation;
        }

        ResetRigidBody();
        EnableAnimator();
    }

    //Reset all enemy settings on respawn
    private void OnEnable()
    {
        ResetRigidBody();
        EnableAnimator();
        if (bones != null)
        {
            ResetBones();
        }
    }

    //put bones back in original position
    public void ResetBones()
    {
        foreach (var bone in bones)
        {
            if (originalPositions.ContainsKey(bone))
            {
                bone.localPosition = originalPositions[bone];
            }
            if (originalRotations.ContainsKey(bone))
            {
                bone.localRotation = originalRotations[bone];
            }
        }
    }

    //resets each ragdoll bone rigid body
    public void ResetRigidBody()
    {
        foreach (var rb in rigidBodies)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Initially disable physics
        }
    }

    public void EnableAnimator()
    {
        enemyAnim.enabled = true;
    }

    public void EnableRagdoll()
    {
        enemyAnim.enabled = false;
        foreach (var rb in rigidBodies)
        {
            rb.isKinematic = false; // Enable physics
        }
    }



    void OnTriggerEnter(Collider other)
    {
        //if enemy is hit by the sword
        if (other.CompareTag("Slash"))
        {
            //get velocity
            Vector3 hitForce = GetRelevantSwordVelocity();

            // if there's a high velocity hit, trigger ragdoll
            if (hitForce.magnitude >= velocityThreshold)
            {
                EnableRagdoll();
                ApplyForce(hitForce, other.transform.position);
            }
        }
    }

    //get the velocity from the appropriate sword
    private Vector3 GetRelevantSwordVelocity()
    {
        //check which hand the sword is in and return the velocity
        if (gunControllerScript.isRightHandActive)
        {
            velocityManager = GameObject.Find("Left Stick").GetComponent<SwordVelocityManager>();
            return velocityManager.GetSwordVelocity() * velocityScaleForce;
        }
        else
        {
            velocityManager = GameObject.Find("Right Stick").GetComponent<SwordVelocityManager>();
            return velocityManager.GetSwordVelocity() * velocityScaleForce;
        }
    }

    //calculate the force to apply at relevant position
    void ApplyForce(Vector3 force, Vector3 hitPoint)
    {
        Vector3 upwardForce = new Vector3(0, force.magnitude * 0.2f, 0);
        Vector3 adjustedForce = force + upwardForce;

        foreach (var rb in rigidBodies)
        {
            rb.AddForceAtPosition(adjustedForce, hitPoint, ForceMode.Impulse);
        }
    }

    //TEST FUNCTIONALITY, NOT PART OF TOPIC 3 - FOR NEXT ITERATION
    public void OnBoneHit(string boneName, Collider other)
    {
        Debug.Log($"Bone hit: {boneName}");
        Vector3 hitForce = GetRelevantSwordVelocity();

        if (boneName == "Leg")
        {
            ApplyForce(hitForce, other.transform.position);
        }
        else
        {
            ApplyForce(hitForce, other.transform.position);
        }
    }

}
