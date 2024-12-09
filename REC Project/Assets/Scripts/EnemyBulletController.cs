using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EnemyBulletController : MonoBehaviour
{
    private float lifetime = 3f; // Lifetime before deactivation
    private float timer;

    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Enemy Target Point").GetComponent<PlayerController>();
    }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slash"))
        {
            DeactivateBullet();
        }
        else if (other.CompareTag("Flames"))
        {
            DeactivateBullet();
        }
        else if (other.CompareTag("Player") && playerControllerScript.damageBufferWait == false)
        {
            playerControllerScript.HealthDamage();
            DeactivateBullet();
        }

    }

        private void DeactivateBullet()
    {
        // Deactivate instead of destroying
        gameObject.SetActive(false);
    }
}
