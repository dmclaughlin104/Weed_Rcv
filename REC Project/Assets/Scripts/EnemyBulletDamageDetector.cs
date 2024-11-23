using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script detects if a enemy bullet has struck the player's head
//Should be attached to the player's head point game object

public class EnemyBulletDamageDetector : MonoBehaviour
{

    private PlayerController playerControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Enemy Target Point").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy Bullet"))
        {
            playerControllerScript.healthCount--;
        }
    }

}
