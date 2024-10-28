using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{

    //variables
    MeshRenderer meshRenderer;
    Color origColour;
    float flashTime = 0.25f;


    // Start is called before the first frame update
    void Start()
    {
        //assigning mesh renderer component
        meshRenderer = GetComponent<MeshRenderer>();

        //assinging current material colour to variable
        origColour = meshRenderer.material.color;

        //begin flashing at a regular rate
        InvokeRepeating("FlashStart", 0,  0.4f);
            
    }

    //method to invoke one flash cycle
    public void FlashStart()
    {
        meshRenderer.material.color = Color.red;
        Invoke("FlashStop", flashTime);
        
    }

    //method to return material to original colour
    public void FlashStop()
    {
        meshRenderer.material.color = origColour;
    }


}
