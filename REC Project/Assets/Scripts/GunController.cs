using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{

    public InputActionReference shoot;
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    private float secondsBetweenShoot = 0.2f;
    float trackshoot;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (shoot.action.ReadValue<float>() == 1 && trackshoot <= 0)
        {
            Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
            trackshoot = secondsBetweenShoot;
        }
        trackshoot -= Time.deltaTime;
    }
}
