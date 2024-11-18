using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWarningPortalController : MonoBehaviour
{

    [SerializeField] private GameObject warningPortal;

    // Start is called before the first frame update
    void Start()
    {
        
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
