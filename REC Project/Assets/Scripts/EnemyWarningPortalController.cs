using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls a 'warning portal' that turns on when enemies are near

public class EnemyWarningPortalController : MonoBehaviour
{

    [SerializeField] GameObject player;
    private float warningDistance = 3f; // Distance to trigger the warning circle
    [SerializeField] private GameObject warningPortal;
    private SpawnManager spawnManagerScript;


    // Start is called before the first frame update
    void Start()
    {
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

    }

    // Update is called once per frame
    void Update()
    {
        // Check for nearby enemies and activate/deactivate the warning circle
        CheckEnemyDistances();
    }


    // Method to check distances and activate/deactivate the warning circle
    void CheckEnemyDistances()
    {
        bool enemyNearby = false;

        foreach (GameObject enemy in spawnManagerScript.activeEnemies)
        {
            if (enemy.activeInHierarchy)
            {
                float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
                if (distance <= warningDistance)
                {
                    enemyNearby = true;
                    break; // No need to check further
                }
            }
        }

        // Activate or deactivate the warning circle based on enemy proximity
        if (warningPortal != null)
        {
            warningPortal.SetActive(enemyNearby);
        }
    }



}
