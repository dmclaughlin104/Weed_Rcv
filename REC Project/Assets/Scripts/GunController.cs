using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    public InputActionReference shootButtonRight;
    public InputActionReference shootButtonLeft;
    public GameObject bulletPrefab;

    [SerializeField] Transform spawnPointLeft;
    [SerializeField] Transform spawnPointRight;
    [SerializeField] GameObject leftMele;
    [SerializeField] GameObject rightMele;
    [SerializeField] Button SwapGunHandButton;

    private InputActionReference activeShootButton;
    private Transform activeBulletSpawn;
    private GameObject activeMele;

    private float secondsBetweenShoot = 0.2f;
    private float trackshoot;
    private bool isRightHandActive = true;

    void Start()
    {
        SetupHand(isRightHandActive);
        SwapGunHandButton.onClick.AddListener(SwapGunHand);
    }

    void Update()
    {
        trackshoot -= Time.deltaTime;
        Shoot(activeShootButton, activeBulletSpawn);
    }

    void SwapGunHand()
    {
        isRightHandActive = !isRightHandActive;
        SetupHand(isRightHandActive);
    }

    void SetupHand(bool rightHandActive)
    {
        if (rightHandActive)
        {
            activeMele = rightMele;
            activeBulletSpawn = spawnPointRight;
            activeShootButton = shootButtonRight;
            leftMele.SetActive(true);
            rightMele.SetActive(false);
        }
        else
        {
            activeMele = leftMele;
            activeBulletSpawn = spawnPointLeft;
            activeShootButton = shootButtonLeft;
            leftMele.SetActive(false);
            rightMele.SetActive(true);
        }
    }

    void Shoot(InputActionReference activeShoot, Transform activePosition)
    {
        if (activeShoot.action.ReadValue<float>() == 1 && trackshoot <= 0)
        {
            Instantiate(bulletPrefab, activePosition.position, activePosition.rotation);
            trackshoot = secondsBetweenShoot;
        }
    }
}
