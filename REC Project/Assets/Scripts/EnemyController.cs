using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    //object variables
    public Animator enemyAnim;
    private Rigidbody enemyRB;
    public GameObject smokeParticle;
    public GameObject weedBloodParticle;
    public GameObject[] enemyBodyParts;
    public SpawnManager spawnManagerScript;
    private Transform player;

    //variables
    private float movementSpeed = 1f;
    private bool isDead = false;
    private float attackForce = 1.5f;
    Vector3 moveDirection;
    private bool isJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        //getting spawnManagerScript component
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        //getting weed animator
        enemyAnim = GetComponent<Animator>();

        //getting rigidbody
        enemyRB = GetComponent<Rigidbody>();

        // Find the player object's transform
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //controlling enemy movement, so long as enemy isn't dead
        if (!isDead && spawnManagerScript.gameActive)
        {
            // Look at the player
            LookAtPlayer();

            // Move toward the player
            MoveTowardsPlayer();
        }

        //trigger roar animation when player is defeated
        if (!spawnManagerScript.gameActive)
        {
            this.enemyAnim.SetBool("playerDead", true);
        }

        //check distance to player to trigger jump animation
        //CheckDistanceToPlayer();


    }

    //method to make enemy look torward player
    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    //method to make enemy move toward player
    void MoveTowardsPlayer()
    {
        // Calculate the movement direction towards the player
        moveDirection = (player.position - transform.position).normalized;

        // Move the enemy towards the player
        transform.Translate(moveDirection * movementSpeed * Time.deltaTime, Space.World);
    }


    //manage triggers
    private void OnTriggerEnter(Collider other)
    {
        //if enemy has been slashed:
        if (other.CompareTag("Slash"))
        {
            //make blood effect start, after brief delay
            //StartCoroutine(bloodDelay());
            
            //trigger enemy death animation
            EnemyDeath();

            //push enemy back on slash attack
            enemyRB.AddForce(-moveDirection * attackForce, ForceMode.Impulse);

            //destroy gameObject after 2 seconds
            Destroy(gameObject, 2);
        }
        //if enemy hit by flames
        else if (other.CompareTag("Flames"))
        {
            //start smoke effect after pause
            StartCoroutine(SmokeDelay());
            
            //trigger enemy death animation
            EnemyDeath();

            //change weed material colour
            ChangeWeedMaterialBlack();

            //destroy gameObject after 4 seconds - longer time to appreciate smoke effect
            Destroy(gameObject, 4f);
        }
        //tigger bite animation if enemy collides with player
        else if (other.CompareTag("Player"))
        {
            this.enemyAnim.SetBool("bitePlayer", true);
            StartCoroutine(BiteCoolDown());
        }

    }

    //trigger jump animation if enemy gets within a certain distance of player
    void CheckDistanceToPlayer()
    {
        float jumpZone = 2f;

        if (Vector3.Distance(transform.position, player.transform.position) < jumpZone)
        {
            enemyAnim.SetBool("inZone", true);
            isJumping = true;

            //test using force for jump
            //enemyRB.AddForce(Vector3.up * jumpForce * Time.deltaTime, ForceMode.Impulse);

            StartCoroutine(JumpingCoolDown());
        }
        //resetting trigger for animation to false when the player gets out of range
        else
        {
            enemyAnim.SetBool("inZone", false);
        }
    }

    //Cooldown Co-routine for biting
    IEnumerator BiteCoolDown()
    {
        yield return new WaitForSeconds(1f);
        this.enemyAnim.SetBool("bitePlayer", false);
    }

    //Cooldown Co-routine for jumping
    IEnumerator JumpingCoolDown()
    {
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    //controlling enemy after death
    void EnemyDeath()
    {
        isDead = true;//stopping enemy movement
        this.enemyAnim.SetBool("isDead", true);
        transform.gameObject.tag = "Dead Enemy";//changing tag so enemy doesn't cause health damage after dying
    }

    //method for changing key parts of enemy to black
    void ChangeWeedMaterialBlack()
    {

        foreach (GameObject bodyPart in enemyBodyParts)
        {
            SkinnedMeshRenderer bodyPartMesh = bodyPart.GetComponent<SkinnedMeshRenderer>();

            bodyPartMesh.material.color = Color.black;

        }
    }

    //waits until enemy is settled on ground before smoke effect starts
    public IEnumerator SmokeDelay()
    {
        yield return new WaitForSeconds(0.9f);
        smokeParticle.SetActive(true);
    }

    //brief delay before blood effect starts
    public IEnumerator bloodDelay()
    {
        yield return new WaitForSeconds(0.25f);
        weedBloodParticle.SetActive(true);
    }

    //waits until enemy is settled on ground it gets swallowed up 
    //not currently used
    public IEnumerator EnemySinkDelay()
    {
        yield return new WaitForSeconds(1.5f);
        transform.Translate(Vector3.down * 15f * Time.deltaTime);
    }


}
