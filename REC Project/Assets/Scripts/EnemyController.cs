using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is applied to every individual enemy in the scene

public class EnemyController : MonoBehaviour
{
    // Object variables
    public Animator enemyAnim;
    private Rigidbody enemyRB;
    public GameObject smokeParticle;
    public GameObject[] enemyBodyParts;
    public SpawnManager spawnManagerScript;
    public GameManager gameManagerScript;
    public AnimationToRagdoll ragdollScript;
    private Transform player;
    public Transform mouthPoint;
    [SerializeField] GameObject playerHeadTarget;

    // Movement variables
    private float movementSpeed = 1f;
    private bool isDead = false;
    private float attackForce = 1.5f;
    Vector3 moveDirection;

    // Shooting variables
    public GameObject enemyBulletPrefab;
    private float shootTimer = 0f; // Timer for shooting intervals
    private float minShootInterval = 5f;
    private float maxShootInterval = 10f;
    private float currentShootInterval;
    private float bulletSpeed;

    private Coroutine releaseBulletCoroutine; // Track the ReleaseBullet coroutine
    private bool isPreparingToShoot = false;

    // Store original colors for each body part
    private Color[] originalColors;

    //audio functionality
    [SerializeField] AudioSource enemyAudioSource;
    [SerializeField] AudioClip[] enemyAliveSounds;
    [SerializeField] AudioClip[] enemyDeathSounds;


    // Start is called before the first frame update
    void Start()
    {
        // Get SpawnManager component
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //getting player head target (i.e. the main camera)
        playerHeadTarget = GameObject.Find("Main Camera").gameObject;

        //getting audioSource
        enemyAudioSource = GetComponent<AudioSource>();
        enemyAudioSource.clip = enemyAliveSounds[0];

        // Initialize other components
        enemyAnim = GetComponent<Animator>();
        enemyRB = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Capture original colors of each body part
        originalColors = new Color[enemyBodyParts.Length];
        for (int i = 0; i < enemyBodyParts.Length; i++)
        {
            SkinnedMeshRenderer bodyPartMesh = enemyBodyParts[i].GetComponent<SkinnedMeshRenderer>();
            originalColors[i] = bodyPartMesh.material.color;
        }

        // Set the initial shooting interval
        SetShootingIntervalByDifficulty();
        currentShootInterval = Random.Range(minShootInterval, maxShootInterval);
    }

    //Start audio immediately upon enemy becoming active
    
    private void OnEnable()
    {
        PlayEnemyAliveSound();
    }
    

    // Update is called once per frame
    void Update()
    {
        SetMovementSpeedByDifficulty();


        if (!isDead && spawnManagerScript.gameActive)
        {
            LookAtPlayer();

            if (!isPreparingToShoot)
            {
                MoveTowardsPlayer();
            }

            shootTimer += Time.deltaTime;
            if (shootTimer >= currentShootInterval && !isPreparingToShoot)
            {
                ShootAtPlayer();
                shootTimer = 0f; // Reset the timer

                // Update the shooting interval
                SetShootingIntervalByDifficulty();
            }
        }

        if (!spawnManagerScript.gameActive)
        {
            this.enemyAnim.SetBool("playerDead", true);
            spawnManagerScript.DeactivateEnemy(gameObject);//enemy calls method to from Spawn Manager to deactivate itself
            ResetEnemyRB();
            ResetEnemy();
        }
    }

    void PlayEnemyAliveSound()
    {
        int randomNum = Random.Range(0, 5);
        enemyAudioSource.clip = enemyAliveSounds[randomNum];
        enemyAudioSource.Play();
    }

    void PlayEnemyDeadSound()
    {
        int randomNum = Random.Range(0, 2);
        enemyAudioSource.Stop();
        //enemyAudioSource.clip = enemyDeathSounds[randomNum];
        enemyAudioSource.PlayOneShot(enemyDeathSounds[randomNum]);
    }


    // Adjust the movement speed based on the difficulty level from SpawnManager
    void SetMovementSpeedByDifficulty()
    {
        if (gameManagerScript.gameDifficulty == GameManager.Difficulty.Easy)
        {
            movementSpeed = 1f; // slower speed for easy
            bulletSpeed = 3f;
        }
        else if (gameManagerScript.gameDifficulty == GameManager.Difficulty.Medium)
        {
            movementSpeed = 1.2f; // medium speed for medium difficulty
            bulletSpeed = 6f;
        }
        else if (gameManagerScript.gameDifficulty == GameManager.Difficulty.Hard)
        {
            movementSpeed = 1.75f; // fastest speed for hard
            bulletSpeed = 9f;
        }
    }

    void SetShootingIntervalByDifficulty()
    {
        if (gameManagerScript.gameDifficulty == GameManager.Difficulty.Easy)
        {
            minShootInterval = 6f;
            maxShootInterval = 12f;
        }
        else if (gameManagerScript.gameDifficulty == GameManager.Difficulty.Medium)
        {
            minShootInterval = 5f;
            maxShootInterval = 10f;
        }
        else if (gameManagerScript.gameDifficulty == GameManager.Difficulty.Hard)
        {
            minShootInterval = 4f;
            maxShootInterval = 8f;
        }
        currentShootInterval = Random.Range(minShootInterval, maxShootInterval);
    }

    //return to a normal state after firing bullet
    IEnumerator ResumeMovementAfterShooting()
    {
        // Wait for the duration of the preparation
        yield return new WaitForSeconds(2.10f);
        this.enemyAnim.SetBool("isShooting", false);
        SetMovementSpeedByDifficulty();
        isPreparingToShoot = false;
    }

    //release bullet at player (in time with animation)
    IEnumerator ReleaseBullet()
    {
        // Pause before releasing bullet
        yield return new WaitForSeconds(1.9f);

        if (isDead) yield break; // Abort if the enemy has died

        GameObject bullet = EnemyBulletPool.Instance.GetBullet();

        if (bullet != null && player != null && mouthPoint != null)
        {
            // Reset bullet position and activate it
            bullet.transform.position = mouthPoint.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.SetActive(true);

            // Calculate the target direction toward the player's head
            Vector3 playerHeadPos = playerHeadTarget.transform.position;

            Vector3 shootDirection = (playerHeadPos - mouthPoint.position).normalized;

            // Set bullet velocity
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = shootDirection * bulletSpeed;
            }
        }
    }

    // Check with ShootingManager if this enemy can shoot
    void ShootAtPlayer()
    {
        if (EnemyShootingManager.Instance != null && EnemyShootingManager.Instance.RequestToShoot())
        {

            // Stop movement and play the "roar" animation
            isPreparingToShoot = true;
            movementSpeed = 0f;
            this.enemyAnim.SetBool("isShooting", true);

            //Coroutines to keep pace with "roar" animation
            StartCoroutine(ResumeMovementAfterShooting());
            releaseBulletCoroutine = StartCoroutine(ReleaseBullet());
        }

    }

    //get player direction
    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    
    void MoveTowardsPlayer()
    {
        moveDirection = (player.position - transform.position).normalized;
        transform.Translate(moveDirection * movementSpeed * Time.deltaTime, Space.World);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slash") && !isDead)
        {
            gameManagerScript.enemiesKilledDuringPlay++;
            EnemyDeath();
            //enemyRB.AddForce(-moveDirection * attackForce, ForceMode.Impulse);
            StartCoroutine(ResetEnemyRB(1.5f));
            StartCoroutine(DeactivateEnemy(2f));

            PlayEnemyDeadSound();

        }
        else if (other.CompareTag("Flames") && !isDead)
        {
            gameManagerScript.enemiesKilledDuringPlay++;
            StartCoroutine(SmokeDelay());
            EnemyDeath();
            ChangeWeedMaterialBlack();
            StartCoroutine(DeactivateEnemy(4f));
            other.gameObject.SetActive(false);

            PlayEnemyDeadSound();

        }
        else if (other.CompareTag("Player"))
        {
            this.enemyAnim.SetBool("bitePlayer", true);
            StartCoroutine(BiteCoolDown());
        }
    }
    

    IEnumerator BiteCoolDown()
    {
        yield return new WaitForSeconds(1f);
        this.enemyAnim.SetBool("bitePlayer", false);
    }


    void EnemyDeath()
    {
        isDead = true;

        // Abort shooting process if in progress
        if (releaseBulletCoroutine != null)
        {
            StopCoroutine(releaseBulletCoroutine);
            releaseBulletCoroutine = null; // Clear reference
        }

        // Reset shooting state and animation
        isPreparingToShoot = false;
        this.enemyAnim.SetBool("isShooting", false);
        this.enemyAnim.SetBool("isDead", true);
        transform.gameObject.tag = "Dead Enemy";
    }

    void ChangeWeedMaterialBlack()
    {
        foreach (GameObject bodyPart in enemyBodyParts)
        {
            SkinnedMeshRenderer bodyPartMesh = bodyPart.GetComponent<SkinnedMeshRenderer>();
            bodyPartMesh.material.color = Color.black;
        }
    }

    public IEnumerator SmokeDelay()
    {
        yield return new WaitForSeconds(0.9f);
        smokeParticle.SetActive(true);
    }

    IEnumerator DeactivateEnemy(float delay)
    {
        yield return new WaitForSeconds(delay);
        spawnManagerScript.DeactivateEnemy(gameObject);
        spawnManagerScript.enemyCount--;
        ResetEnemy();
    }

    public void ResetEnemyRB()
    {
        enemyRB.velocity = Vector3.zero;
        enemyRB.angularVelocity = Vector3.zero;

    }

    IEnumerator ResetEnemyRB(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyRB.velocity = Vector3.zero;
        enemyRB.angularVelocity = Vector3.zero;
    }

    public void ResetEnemy()
    {
        isDead = false;
        this.enemyAnim.SetBool("isDead", false);
        this.enemyAnim.SetBool("playerDead", false);
        this.enemyAnim.SetBool("bitePlayer", false);
        this.enemyAnim.SetBool("inZone", false);
        this.enemyAnim.SetBool("isShooting", false);
        transform.gameObject.tag = "Weed Enemy";
        smokeParticle.SetActive(false);

        // Restore original colors
        for (int i = 0; i < enemyBodyParts.Length; i++)
        {
            SkinnedMeshRenderer bodyPartMesh = enemyBodyParts[i].GetComponent<SkinnedMeshRenderer>();
            bodyPartMesh.material.color = originalColors[i];
        }
    }
}
