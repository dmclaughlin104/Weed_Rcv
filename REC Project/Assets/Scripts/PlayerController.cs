using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Main Player Controller script
public class PlayerController : MonoBehaviour
{

    //[SerializeField] GameObject damageIndicator;
    private SpawnManager spawnManagerScript;
    [SerializeField] AudioSource playerAudio;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip flamethrowerSound;

    //variables
    public bool hasPowerUp = false;
    private bool damageBufferWait = false;
    public int healthCount = 3;
    public int maxHealth = 3;
    private float damageBufferTime = 2.5f;


    // Start is called before the first frame update
    void Start()
    {
        //finding Spawn Manager in order to take wave number variable
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        //getting player audio
        playerAudio = GetComponent<AudioSource>();

        //setting value for flame-thrower slider
        //flameThrowerSlider.maxValue = flamethrowerTime;

    }

    // Update is called once per frame
    void Update()
    {

        


    }



    //actions if triggers are activated
    private void OnTriggerEnter(Collider other)
    {
        //if player picks up power up, start flame-thrower
        if (other.CompareTag("PowerUp") && hasPowerUp == false)
        {         
            Destroy(other.gameObject);//destroy power-up
        }//if
        //or if player is struck by enemy
        else if (other.CompareTag("Weed Enemy") && damageBufferWait == false)
        {
            HealthDamage();
            //UnityEngine.Debug.Log("Buffer wait = " + damageBufferWait + " ...and should be True");
        }//else if
    }
    

    //method for depleting health
    void HealthDamage()
    {
        healthCount--;
        //playerAudio.PlayOneShot(hurtSound);
        damageBufferWait = true;
        StartCoroutine(DamageBufferCountdown());

        //turn on damage indicator icon as long as player is still alive
        if (healthCount > 0)
        {
            //damageIndicator.gameObject.SetActive(true);
        }
        //if health is fully depleted, play deathbell sound
        else if (healthCount == 0)
        {
            //playerAudio.PlayOneShot(deathSound);
        }
    }

    //damage buffer method to limit the amount of health you can lose in a short period
    IEnumerator DamageBufferCountdown()
    {
        yield return new WaitForSeconds(damageBufferTime);
        damageBufferWait = false;
        //damageIndicator.gameObject.SetActive(false);
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
