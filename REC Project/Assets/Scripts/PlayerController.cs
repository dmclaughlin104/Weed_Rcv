using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Main Player Controller script
public class PlayerController : MonoBehaviour
{
    private SpawnManager spawnManagerScript;

    //variables
    public bool hasPowerUp = false;
    public bool damageBufferWait = false;
    public int healthCount = 3;
    public int maxHealth = 3;
    private float damageBufferTime = 2.5f;


    // Start is called before the first frame update
    void Start()
    {
        //finding Spawn Manager in order to take wave number variable
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

    }


    //actions if triggers are activated
    private void OnTriggerEnter(Collider other)
    {
        
        //if player is struck by enemy
        if (other.CompareTag("Weed Enemy") && damageBufferWait == false)
        {
            HealthDamage();
            //UnityEngine.Debug.Log("Enemy Strike");
        }
        
    }
    

    //method for depleting health
    public void HealthDamage()
    {
        healthCount--;
        damageBufferWait = true;
        StartCoroutine(DamageBufferCountdown());
    }

    //damage buffer method to limit the amount of health you can lose in a short period
    IEnumerator DamageBufferCountdown()
    {
        yield return new WaitForSeconds(damageBufferTime);
        damageBufferWait = false;
    }

    //method to reset health to full
    public void ResetHealth()
    {
        healthCount = maxHealth;
    }
}   
