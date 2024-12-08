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

    private SwordVelocityManager velocityManager;


    // Start is called before the first frame update
    void Start()
    {

        //get script
        velocityManager = GameObject.Find("Left Stick").GetComponent<SwordVelocityManager>();

        //get all rigid bodies
        rigidBodies = GetComponentsInChildren<Rigidbody>();

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

    // Update is called once per frame
    void Update()
    {

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
            //Debug.Log($"Triggered by: {other.name}, Rigidbody attached: {other.attachedRigidbody != null}");



            Vector3 hitForce = velocityManager.GetSwordVelocity() * 10f; // Scale force
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