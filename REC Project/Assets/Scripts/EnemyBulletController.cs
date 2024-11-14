using UnityEngine;
using System.Collections;

public class EnemyBulletController : MonoBehaviour
{
    private float lifetime = 3f; // Lifetime before deactivation
    private float timer;

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            DeactivateBullet();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        DeactivateBullet();

    }


    private void DeactivateBullet()
    {
        // Deactivate instead of destroying
        gameObject.SetActive(false);
    }
}
