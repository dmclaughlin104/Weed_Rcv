using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//one rotation script that can serve my power-up as well as my flame objects
public class ObjectRotate : MonoBehaviour
{
    //variables
    public float rotateSpeed;
    public float xAxis;
    public float yAxis;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //rotating object at constant rate
        transform.Rotate(new Vector3(xAxis, yAxis, 0) * rotateSpeed * Time.deltaTime);
    }
}
