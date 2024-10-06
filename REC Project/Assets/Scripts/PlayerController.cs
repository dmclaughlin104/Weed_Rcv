using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Main Player Controller script
public class PlayerController : MonoBehaviour
{
    //player animator variable
    public Animator playerAnim;

    //game objets, sounds & UI variables
    [SerializeField] GameObject attackObject;
    [SerializeField] GameObject flamesBox;//flames object with box collider
    [SerializeField] GameObject flamesObject;//flames affect in scene
    public GameObject grave;
    [SerializeField] GameObject damageIndicator;
    private SpawnManager spawnManagerScript;
    [SerializeField] AudioSource playerAudio;
    [SerializeField] AudioClip slashSound;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip flamethrowerSound;
    [SerializeField] Slider flameThrowerSlider;
    [SerializeField] ParticleSystem slashParticle;
    [SerializeField] Slider flamethrowerBar;
    [SerializeField] TextMeshProUGUI flamethrowerText;

    //variables
    private float forwardSpeed = 5.0f;
    private float turnSpeed = 150.0f;
    private float xBoundary = 8;
    private float zBoundary = 7;
    public bool hasPowerUp = false;
    private bool damageBufferWait = false;
    private bool isSlashing = false;
    public int healthCount = 3;
    public int maxHealth = 3;
    private float flamethrowerTime = 4.0f;
    private float damageBufferTime = 2.5f;


    // Start is called before the first frame update
    void Start()
    {
        //finding Spawn Manager in order to take wave number variable
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        //getting player audio
        playerAudio = GetComponent<AudioSource>();

        //setting value for flame-thrower slider
        flameThrowerSlider.maxValue = flamethrowerTime;

    }

    // Update is called once per frame
    void Update()
    {

        //flame-throwerUI counter
        //N.B this doesn't work as expected inside other methods
        //needs to be constantly called in Update() to give gradual decreasing effect

        if (hasPowerUp == true)
        {
            FlameThrowerUIActive();
        }
        else
        {
            FlameThrowerUINotActive();
        }

        //if game is live, player can move
        if (spawnManagerScript.gameActive)
        {
            MovementControls();
            PlayerBoundaryControls();
        }

        //detect if player is attacking/slashing, if appropriate
        if (Input.GetKeyDown(KeyCode.Space) && !isSlashing && !hasPowerUp && spawnManagerScript.gameActive)
        {
            //playing slash sound effect
            //here rather than in method in an attempt to reduce lag
            playerAudio.PlayOneShot (slashSound);

            //carry out slash
            SlashEffect();
            StartCoroutine(SlashEndCountdown());
        }


    }

    //method which activates player movement controls
    void MovementControls()
    {
        //assigning axis inputs to float variables
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        
        //moving player
        transform.Translate(Vector3.forward * forwardInput * forwardSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up * horizontalInput * turnSpeed * Time.deltaTime);

        //assigning axis values to animation controller
        this.playerAnim.SetFloat("vertical", forwardInput);
        this.playerAnim.SetFloat("horizontal", horizontalInput);

    }

    //keeping player within bounds
    void PlayerBoundaryControls()
    {
        //if statement to control left player boundary
        if (transform.position.x < -xBoundary)
        {
            transform.position = new Vector3(-xBoundary, transform.position.y, transform.position.z);
        }//if

        //if statement to control right player boundary
        if (transform.position.x > xBoundary)
        {
            transform.position = new Vector3(xBoundary, transform.position.y, transform.position.z);
        }//if

        //if statement to control player Z-axis upper boundary
        if (transform.position.z > zBoundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zBoundary);
        }//if

        //if statement to control player Z-axis lower boundary
        if (transform.position.z < -zBoundary)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -zBoundary);
        }//if
    }

    //carry out player's slash attack
    void SlashEffect()
    {
        isSlashing = true;
        attackObject.SetActive(true);//make slash object live in scene
        slashParticle.Play();//play particle effect
        this.playerAnim.SetBool("isSlashing", true);//play animation
    }

    //ends the slash attack
    IEnumerator SlashEndCountdown()
    {
        yield return new WaitForSeconds(0.2f);
        attackObject.SetActive(false);
        this.playerAnim.SetBool("isSlashing", false);
        isSlashing = false;

    }

    //actions if triggers are activated
    private void OnTriggerEnter(Collider other)
    {
        //if player picks up power up, start flame-thrower
        if (other.CompareTag("PowerUp") && hasPowerUp == false)
        {         
            Destroy(other.gameObject);//destroy power-up
            TurnOnFlameThrower();
        }//if
        //or if player is struck by enemy
        else if (other.CompareTag("Weed Enemy") && damageBufferWait == false)
        {
            HealthDamage();
            //UnityEngine.Debug.Log("Buffer wait = " + damageBufferWait + " ...and should be True");
        }//else if
    }

    //method to turn on flamethrower effects and begin Coroutine
    void TurnOnFlameThrower()
    {
        flamesBox.SetActive(true);
        flamesObject.SetActive(true);
        
        flameThrowerSlider.value = flamethrowerTime;//setting UI back to full
        hasPowerUp = true;
        this.playerAnim.SetBool("isFlamethrowing", true);
        playerAudio.PlayOneShot(flamethrowerSound);
        StartCoroutine(FlamethrowerCountdown());
    }

    //ends the flamethrower power-up
    IEnumerator FlamethrowerCountdown()
    {
        yield return new WaitForSeconds(flamethrowerTime);
        flamesBox.SetActive(false);
        flamesObject.SetActive(false);
        hasPowerUp = false;
        this.playerAnim.SetBool("isFlamethrowing", false);
    }

    //A flamethrower timer UI countdown
    //needs to be called separately to other flamethrower effects to function as expected
    //flamethrower slider value gradually decreased to 0 over 4 seconds;
    //flamethrower UI controller
    void FlameThrowerUIActive()
    {
        flameThrowerSlider.value -= 1 * Time.deltaTime;
        flamethrowerBar.gameObject.SetActive(true);
        flamethrowerText.gameObject.SetActive(true);
    }

    void FlameThrowerUINotActive()
    {
        flamethrowerBar.gameObject.SetActive(false);
        flamethrowerText.gameObject.SetActive(false);
    }


    //method for depleting health
    void HealthDamage()
    {
        healthCount--;
        playerAudio.PlayOneShot(hurtSound);
        damageBufferWait = true;
        StartCoroutine(DamageBufferCountdown());

        //turn on damage indicator icon as long as player is still alive
        if (healthCount > 0)
        {
            damageIndicator.gameObject.SetActive(true);
        }
        //if health is fully depleted, play deathbell sound
        else if (healthCount == 0)
        {
            playerAudio.PlayOneShot(deathSound);
        }
    }

    //damage buffer method to limit the amount of health you can lose in a short period
    IEnumerator DamageBufferCountdown()
    {
        yield return new WaitForSeconds(damageBufferTime);
        damageBufferWait = false;
        damageIndicator.gameObject.SetActive(false);
        //UnityEngine.Debug.Log("Buffer wait = " + damageBufferWait + " ...and should be false");
    }

    //method to reset health to full
    public void ResetHealth()
    {
        healthCount = maxHealth;
    }


    //method for debugging
    void PrintHealthToDebugLog()
    {
        //printing current health to debug log
        if (healthCount > 0)
        {
            UnityEngine.Debug.Log("Your health = " + healthCount + "/3. You are on wave = " + spawnManagerScript.nextWave);
        }
        else
        {
            UnityEngine.Debug.Log("You've died - Game Over. You reached wave " + spawnManagerScript.nextWave);
        }
    }


}   
