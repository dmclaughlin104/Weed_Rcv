using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Object variables
    public Animator enemyAnim;
    private Rigidbody enemyRB;
    public GameObject smokeParticle;
    public GameObject[] enemyBodyParts;
    public SpawnManager spawnManagerScript;
    private Transform player;

    // Movement variables
    private float movementSpeed = 1f;
    private bool isDead = false;
    private float attackForce = 1.5f;
    Vector3 moveDirection;
    private bool isJumping = false;

    // Store original colors for each body part
    private Color[] originalColors;

    // Start is called before the first frame update
    void Start()
    {
        // Get SpawnManager component
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

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
    }

    // Adjust the movement speed based on the difficulty level from SpawnManager
    void SetMovementSpeedByDifficulty()
    {
        if (spawnManagerScript.gameDifficulty == SpawnManager.Difficulty.Easy)
        {
            movementSpeed = 1f; // slower speed for easy
        }
        else if (spawnManagerScript.gameDifficulty == SpawnManager.Difficulty.Medium)
        {
            movementSpeed = 1.2f; // medium speed for medium difficulty
        }
        else if (spawnManagerScript.gameDifficulty == SpawnManager.Difficulty.Hard)
        {
            movementSpeed = 1.75f; // fastest speed for hard
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Adjust enemy speed based on difficulty
        SetMovementSpeedByDifficulty();

        if (!isDead && spawnManagerScript.gameActive)
        {
            LookAtPlayer();
            MoveTowardsPlayer();
        }

        if (!spawnManagerScript.gameActive)
        {
            this.enemyAnim.SetBool("playerDead", true);
            spawnManagerScript.DeactivateEnemy(gameObject);
            ResetEnemyRB();
            ResetEnemy();
        }
    }

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
        if (other.CompareTag("Slash"))
        {
            EnemyDeath();
            enemyRB.AddForce(-moveDirection * attackForce, ForceMode.Impulse);
            StartCoroutine(ResetEnemyRB(1.5f));
            StartCoroutine(DeactivateEnemy(2f));
        }
        else if (other.CompareTag("Flames"))
        {
            StartCoroutine(SmokeDelay());
            EnemyDeath();
            ChangeWeedMaterialBlack();
            StartCoroutine(DeactivateEnemy(4f));
            other.gameObject.SetActive(false);
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

    IEnumerator JumpingCoolDown()
    {
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    void EnemyDeath()
    {
        isDead = true;
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
