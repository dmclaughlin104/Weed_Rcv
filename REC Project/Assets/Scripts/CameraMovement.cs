using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


//This method makes the camera function as if it were a child object of the player
//I'd intially thought about taking the player out of the scene upon game-over and didn't want the camera also being removed.
//This method was the result of some googling and trial and error to find the right position.
//It is working, but I'd like to research further how each element functions.

public class CameraMovement : MonoBehaviour
{
    //variables
    public GameObject player;
    private Vector3 cameraPosition;
    private float yChange =-1f;
    private float positionMultiplier = 5f;



    // Update is called once per frame
    void LateUpdate()
    {

        //asigning player position/movement to camera object
        cameraPosition = player.transform.forward;

        //altering the camera Y position
        cameraPosition.y = yChange;
        
        //determining the position of the camera behind the player
        transform.position = player.transform.position - cameraPosition * positionMultiplier;//some trial and error to find right position

        //making camera face same direction as player
        transform.forward = player.transform.position - transform.position;

    }


}
