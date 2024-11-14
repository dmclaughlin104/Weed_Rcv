using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletPool : MonoBehaviour

{
    public static EnemyBulletPool Instance; // Singleton instance
    [SerializeField] GameObject bulletPrefab;    // Prefab for the bullet
    private int poolSize = 4;          // Total number of bullets in the pool

    private List<GameObject> bulletPool;

    void Awake()
    {
        // Initialize singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize bullet pool
        InitializeBulletPool();
    }

    // Method to initialize the bullet pool
    private void InitializeBulletPool()
    {
        bulletPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false); // Deactivate bullets at start
            bulletPool.Add(bullet);
        }
    }

    // Method to get an inactive bullet from the pool
    public GameObject GetBullet()
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy) // Find an inactive bullet
            {
                return bullet;
            }
        }
        return null; // Return null if no inactive bullets are available
    }
}

