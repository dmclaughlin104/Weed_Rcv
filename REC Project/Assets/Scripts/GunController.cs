using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    // Shooting input references
    public InputActionReference shootButtonRight;
    public InputActionReference shootButtonLeft;
    private InputActionReference activeShootButton;

    // Bullet settings
    public GameObject bulletPrefab;
    [SerializeField] Transform spawnPointLeft;
    [SerializeField] Transform spawnPointRight;
    private Transform activeBulletSpawn;
    private float shootForce = 1000f;
    private float bulletRemainTime = 3.0f;
    private float secondsBetweenShoot = 0.2f;
    private float trackshoot;

    // Ammo and pool settings
    [SerializeField] int maxAmmo = 10;
    private int currentAmmo;
    private float ammoRegenCooldown = 1f;
    private List<GameObject> bulletPool;
    private bool isRightHandActive = true;

    // Melee and Gun objects for swapping
    [SerializeField] GameObject leftMele;
    [SerializeField] GameObject rightMele;
    [SerializeField] GameObject leftGun;
    [SerializeField] GameObject rightGun;
    [SerializeField] GameObject rightControllerMesh;
    [SerializeField] GameObject leftControllerMesh;
    [SerializeField] Button swapGunHandButton;

    // UI
    [SerializeField] Slider ammoBarLeft;
    [SerializeField] Slider ammoBarRight;
    [SerializeField] GameObject leftArmCanvas;
    [SerializeField] GameObject rightArmCanvas;

    void Start()
    {
        // Initialize ammo
        currentAmmo = maxAmmo;
        InitializeBulletPool();
        UpdateAmmoBar();

        // Setup hand and ammo regeneration coroutine
        SetupHand(isRightHandActive);
        swapGunHandButton.onClick.AddListener(SwapGunHand);
        StartCoroutine(RegenerateAmmo());

    }

    void Update()
    {
        trackshoot -= Time.deltaTime;
        Shoot(activeShootButton, activeBulletSpawn);

    }

    // Set up bullet pool
    void InitializeBulletPool()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < maxAmmo; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }

    // Shoot if ammo is available and cooldown has elapsed
    void Shoot(InputActionReference activeShoot, Transform activePosition)
    {
        if (activeShoot.action.ReadValue<float>() == 1 && trackshoot <= 0 && currentAmmo > 0)
        {
            GameObject bullet = GetBulletFromPool();
            if (bullet != null)
            {
                bullet.transform.position = activePosition.position;
                bullet.transform.rotation = activePosition.rotation;
                bullet.SetActive(true);

                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(activePosition.forward * shootForce);

                currentAmmo--;
                UpdateAmmoBar();
                trackshoot = secondsBetweenShoot;

                // Start the lifetime countdown
                StartCoroutine(LifetimeCountdown(bullet));
            }
        }
    }

    // Retrieve an inactive bullet from the pool
    GameObject GetBulletFromPool()
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }
        return null; // No bullets available in the pool
    }

    // Swap between hands
    void SwapGunHand()
    {
        isRightHandActive = !isRightHandActive;
        SetupHand(isRightHandActive);
    }

    // Set up active hand settings
    void SetupHand(bool rightHandActive)
    {
        if (rightHandActive)
        {
            activeBulletSpawn = spawnPointRight;
            activeShootButton = shootButtonRight;
            leftMele.SetActive(true);
            rightMele.SetActive(false);
            leftGun.SetActive(false);
            rightGun.SetActive(true);
            rightControllerMesh.SetActive(false);
            leftControllerMesh.SetActive(true);
            leftArmCanvas.SetActive(false);
            rightArmCanvas.SetActive(true);
        }
        else
        {
            activeBulletSpawn = spawnPointLeft;
            activeShootButton = shootButtonLeft;
            leftMele.SetActive(false);
            rightMele.SetActive(true);
            leftGun.SetActive(true);
            rightGun.SetActive(false);
            rightControllerMesh.SetActive(true);
            leftControllerMesh.SetActive(false);
            leftArmCanvas.SetActive(true);
            rightArmCanvas.SetActive(false);
        }
    }

    // Coroutine for regenerating ammo over time
    private IEnumerator RegenerateAmmo()
    {
        while (true)
        {
            yield return new WaitForSeconds(ammoRegenCooldown);

            if (currentAmmo < maxAmmo)
            {
                currentAmmo++;
                UpdateAmmoBar();
            }
        }
    }

    private IEnumerator LifetimeCountdown(GameObject bullet)
    {
        yield return new WaitForSeconds(bulletRemainTime);
        bullet.SetActive(false); // Return to pool
    }

    // Update ammo bar
    private void UpdateAmmoBar()
    {
        if (ammoBarLeft != null || ammoBarRight != null)
        {
            ammoBarLeft.maxValue = maxAmmo;
            ammoBarLeft.value = currentAmmo;
            ammoBarRight.maxValue = maxAmmo;
            ammoBarRight.value = currentAmmo;
        }
    }
}
