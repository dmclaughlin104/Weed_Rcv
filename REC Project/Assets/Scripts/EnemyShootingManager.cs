using UnityEngine;

public class EnemyShootingManager : MonoBehaviour
{
    public static EnemyShootingManager Instance;

    // Time in seconds between shots by different enemies
    private float minimumDelayBetweenShots = 10f;
    private float shootCooldownTimer = 0f;
    private bool canShoot = true;

    void Awake()
    {
        // Singleton pattern to ensure one instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Update the cooldown timer if shooting is disabled
        if (!canShoot)
        {
            shootCooldownTimer -= Time.deltaTime;
            if (shootCooldownTimer <= 0f)
            {
                canShoot = true; // Allow shooting again when cooldown expires
            }
        }
    }

    // Method to check if an enemy can shoot, i.e. whether enough time has passed
    public bool RequestToShoot()
    {
        if (canShoot)
        {
            canShoot = false; // Disable shooting until cooldown completes
            shootCooldownTimer = minimumDelayBetweenShots; // Reset cooldown
            return true; // Permit the current enemy to shoot
        }
        return false; // Deny request if within cooldown
    }
}
