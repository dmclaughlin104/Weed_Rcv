using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToRagdoll : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Collider myCollider;
    [SerializeField] Rigidbody[] rigidBodies;

    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();
    private Transform[] bones;
    private float velocityScaleForce = 1.5f;

    private SwordVelocityManager velocityManager;
    private GunController gunControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        // Get all rigid bodies
        rigidBodies = GetComponentsInChildren<Rigidbody>();

        //get script
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

    private void OnEnable()
    {
        ResetRigidBody();
        EnableAnimator();
        if (bones != null)
        {
            ResetBones();
        }
    }

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
        animator.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slash"))
        {
            Rigidbody swordRb = other.attachedRigidbody;

            if (swordRb != null)
            {
                Vector3 hitForce = GetRelevantSwordVelocity();
                EnableRagdoll();
                ApplyForce(hitForce, other.transform.position);
            }
        }
    }

    //getting velocity hitforce from current hand/weapon orientation
    private Vector3 GetRelevantSwordVelocity()
    {
        if (gunControllerScript.isRightHandActive)
        {
            velocityManager = GameObject.Find("Left Stick").GetComponent<SwordVelocityManager>();
            return velocityManager.GetSwordVelocity() * velocityScaleForce;
        }
        else
            velocityManager = GameObject.Find("Right Stick").GetComponent<SwordVelocityManager>();
            return velocityManager.GetSwordVelocity() * velocityScaleForce; ;
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
        // Optionally add upward direction to make the ragdoll fall back more dramatically
        Vector3 upwardForce = new Vector3(0, force.magnitude * 0.2f, 0);
        Vector3 adjustedForce = force + upwardForce;

        // Apply force to all rigidbodies in the ragdoll for a more natural movement
        foreach (var rb in rigidBodies)
        {
            rb.AddForceAtPosition(adjustedForce, hitPoint, ForceMode.Impulse);
        }

    }

}
