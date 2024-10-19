using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetController : MonoBehaviour
{

    public GameObject player;
    private float fixedHeight = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        // Get the player's position
        Vector3 playerPosition = player.transform.position;

        // Set the new position of the object to match the player's X and Z, but use the specified fixed height for Y
        gameObject.transform.position = new Vector3(playerPosition.x, fixedHeight, playerPosition.z);
    }
}
