using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessingEffects : MonoBehaviour
{
    [SerializeField] private GameObject postProcessingVolume; // Reference to the Post-Processing Volume

    public void OnPlayerDeath()
    {
        // Enable the grey tint effect
        postProcessingVolume.SetActive(true);

    }

    public void OnGameRestart()
    {
        // Disable the grey tint effect
        postProcessingVolume.SetActive(false);

    }
}
