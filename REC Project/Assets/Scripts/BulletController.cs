using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float shootForce = 1000f;
    private Rigidbody bulletRigidBody;
    private float timeToLive = 3f;

    // Called when the bullet is activated
    public void ActivateBullet(Vector3 position, Quaternion rotation)
    {
        // Reset position and rotation
        transform.position = position;
        transform.rotation = rotation;

        // Reset physics and apply shooting force
        bulletRigidBody = GetComponent<Rigidbody>();
        bulletRigidBody.velocity = Vector3.zero;
        bulletRigidBody.angularVelocity = Vector3.zero;
        bulletRigidBody.AddForce(transform.forward * shootForce);

        // Start the lifetime countdown
        StartCoroutine(LifetimeCountdown());
    }

    // Recycle bullet after it has expired
    private IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(timeToLive);
        gameObject.SetActive(false); // Return to pool
    }
}
