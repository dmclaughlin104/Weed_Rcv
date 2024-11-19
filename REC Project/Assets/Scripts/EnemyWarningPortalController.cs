using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weed Enemy"))
        {
            warningPortal.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        warningPortal.SetActive(false);
    }

}
