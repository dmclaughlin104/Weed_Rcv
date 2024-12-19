using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOT IN USE YET - ATTEMPTING TO GET DIFFICULTY SETTINGS OUT OF ENEMY CONTROLLER UPDATE METHOD
public class EnemyDifficultySettings : MonoBehaviour
{
    private GameManager gameManagerScript;
    public bool easyMode;
    public bool mediumMode;
    public bool hardMode;


    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void Easy()
    {
        easyMode = true;
        mediumMode = false;
        hardMode = false;    
    }

    public void Medium()
    {
        easyMode = false;
        mediumMode = true;
        hardMode = false;
    }

    public void Hard()
    {
        easyMode = false;
        mediumMode = false;
        hardMode = true;
    }

}
