using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordVelocityManager : MonoBehaviour
{

    private Vector3 lastPosition;
    private Vector3 currentVelocity;

    // Start is called before the first frame update
    void Start()
    {

    }


    void Update()
    {
        // Calculate velocity as change in position per frame
        Vector3 currentPosition = transform.position;
        currentVelocity = (currentPosition - lastPosition) / Time.deltaTime;
        lastPosition = currentPosition;
    }

    // Access the velocity in OnTriggerEnter
    public Vector3 GetSwordVelocity()
    {
        return currentVelocity;
    }

}