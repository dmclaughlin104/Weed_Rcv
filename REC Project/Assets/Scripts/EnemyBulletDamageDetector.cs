using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
