using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToRagdoll : MonoBehaviour
{
    [SerializeField] Animator enemyAnim;
    [SerializeField] Collider myCollider;
    [SerializeField] Rigidbody[] rigidBodies;

    [SerializeField] float velocityThreshold = 15f; // Threshold to switch between animation and ragdoll death state
    [SerializeField] float velocityScaleForce = 1.5f;

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
        enemyAnim.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slash"))
        {
            Vector3 hitForce = GetRelevantSwordVelocity();

            if (hitForce.magnitude >= velocityThreshold)
            {
                // High velocity: Ragdoll mode
                EnableRagdoll();
                ApplyForce(hitForce, other.transform.position);
            }
            /*
            else
            {
                // Low velocity: Animated fall
                TriggerFallAnimation(hitForce);
            }
            */
        }
    }



    private Vector3 GetRelevantSwordVelocity()
    {
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

    void EnableRagdoll()
    {
        enemyAnim.enabled = false;
        foreach (var rb in rigidBodies)
        {
            rb.isKinematic = false; // Enable physics
        }
    }

    void ApplyForce(Vector3 force, Vector3 hitPoint)
    {
        Vector3 upwardForce = new Vector3(0, force.magnitude * 0.2f, 0);
        Vector3 adjustedForce = force + upwardForce;

        foreach (var rb in rigidBodies)
        {
            rb.AddForceAtPosition(adjustedForce, hitPoint, ForceMode.Impulse);
        }
    }


    void TriggerFallAnimation(Vector3 hitForce)
    {
        // Ensure the animator is enabled
        EnableAnimator();

        // Optionally rotate the enemy to simulate falling opposite the hit
        Vector3 fallDirection = -hitForce.normalized;
        Quaternion fallRotation = Quaternion.LookRotation(new Vector3(fallDirection.x, 0, fallDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, fallRotation, 0.5f);

        this.enemyAnim.SetBool("isDead", true);
    }
}
