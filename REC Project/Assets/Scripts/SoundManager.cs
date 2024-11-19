using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    //audio functionality
    public AudioSource enemyAudioSource;
    private AudioClip enemyAliveSound;
    private AudioClip enemyAliveSound2;
    private AudioClip enemyDeathSound1;
    private AudioClip enemyDeathSound2;


    // Start is called before the first frame update
    void Start()
    {
        //get AudioSource
        enemyAudioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayEnemyAliveSound()
    {
        enemyAudioSource.Play();
    }


}
