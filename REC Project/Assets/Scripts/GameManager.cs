using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    //player animator
    [SerializeField] Animator playerAnim;

    //UI elements
    [SerializeField] TextMeshProUGUI titleScreen;
    [SerializeField] Button startButton;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI gameOver1;
    [SerializeField] TextMeshProUGUI gameOver2;
    [SerializeField] RawImage gameOverTint;
    [SerializeField] TextMeshProUGUI timerText;


    //variables
    private PlayerController playerControllerScript;
    private SpawnManager spawnManagerScript;
    private GameObject[] enemies;
    private GameObject[] powerUps;
    private float secondsCount;
    private int minuteCount;



    // Start is called before the first frame update
    void Start()
    {
        //finding scripts
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        //adding listener to button
        startButton.onClick.AddListener(StartGame);
    }

    // Update is called once per frame
    void Update()
    {
        //debug method for quickly skipping through levels
        //KillWaveDebug();


        //finding enemies and power-ups for arrays
        //needs to be in update to keep up to date
        enemies = GameObject.FindGameObjectsWithTag("Weed Enemy");
        powerUps = GameObject.FindGameObjectsWithTag("PowerUp");

        //updating UI and health variables
        //UpdateWaveText(spawnManagerScript.nextWave);
        HealthManager(playerControllerScript.healthCount);

        //activating UI gameplay timer
        if (spawnManagerScript.gameActive)
        {
            UpdateTimerUI();
        }


    }

    //method to start game...
    void StartGame()
    {
        //Debug.Log("Button clicked");

        //telling spawn manager that game is active
        spawnManagerScript.gameActive = true;

        /*
        //set grave gameObject inactive
        playerControllerScript.grave.gameObject.SetActive(false);

        //turning off death animation
        this.playerAnim.SetBool("isDead", false);

        //resetting UI timer to 00:00;
        minuteCount = 0;
        secondsCount = 0;

        
        //activating & deactivating UI elements
        healthText.gameObject.SetActive(true);
        waveText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);

        gameOver1.gameObject.SetActive(false);
        gameOver2.gameObject.SetActive(false);
        gameOverTint.gameObject.SetActive(false);
        titleScreen.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        */

    }

    /*
    //method to call the game over screen and reset elements for next play
    void GameOverScreen()
    {

        //stopping spawning by making game inactive
        spawnManagerScript.gameActive = false;

        //turn on grave object
        playerControllerScript.grave.gameObject.SetActive(true);

        //ensuring player stops running during Game Over screen:
        this.playerAnim.SetFloat("vertical", 0);

        //assigning final wave number
        //not necessarily needed as I could simply call the script variable
        //retained in case useful for leaderboard
        int gameOverWaveNumber = spawnManagerScript.nextWave;

        //turning off relevant UI
        healthText.gameObject.SetActive(false);
        waveText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);

        //turning on and updating relevant UI
        gameOver2.text = "You reached" + "\n" + "wave " + gameOverWaveNumber + "\n in \n" +
            minuteCount + " mins " + (int)secondsCount + " secs";
        gameOver1.gameObject.SetActive(true);
        gameOver2.gameObject.SetActive(true);
        gameOverTint.gameObject.SetActive(true);

        //brief pause before start button appears
        StartCoroutine(RestartButtonPause());

        //destroying all remaining enemies at the end of the game
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy, 3.5f);//waiting so enemy roar animation can play
        }

        //destroying all remaining power-ups at the end of the game
        foreach (GameObject powerUp in powerUps)
        {
            Destroy(powerUp);
        }
    }
    */
    /*

    //stipulating a brief pause on Game Over before restart button appears
    IEnumerator RestartButtonPause()
    {
        yield return new WaitForSeconds(3.5f);
        startButton.gameObject.SetActive(true);
    }
    */

    
    //reset game elements for next play;
    void ResetForNextPlay()
    {
        playerControllerScript.ResetHealth();
        spawnManagerScript.ResetNextWave();
    }

    /*
    //method to keep wave text up to date
    public void UpdateWaveText(int pWaveNo)
    {
        waveText.text = "Wave: " + pWaveNo;
    }
    */

    //method to keep health text up to date
    public void HealthManager(int pHealthNo)
    {
        if (playerControllerScript.healthCount > 0)
        {
            healthText.text = "Health: " + pHealthNo + "/3";
        }
        else
        {
            //GameOverScreen();
            ResetForNextPlay();
            this.playerAnim.SetBool("isDead", true);
        }
    }

    //method to manage a UI timer
    void UpdateTimerUI()
    {
        //set timer UI
        //second count follows deltaTime
        secondsCount += Time.deltaTime;

        //formatting string for UI
        timerText.text = string.Format("{0:00}:{1:00}", minuteCount, secondsCount);

        //count to 60 seconds, then add minute to counter and reset seconds
        if (secondsCount >= 60)
        {
            minuteCount++;
            secondsCount = 0;
        }

    }

    //Debugging method to progress through waves
    void KillWaveDebug()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }
        }
    }

    /*
    //flamethrower UI controller
    void FlameThrowerUIActive()
    {
        flamethrowerBar.gameObject.SetActive(true);
        flamethrowerText.gameObject.SetActive(true);
    }

    void FlameThrowerUINotActive()
    {
        flamethrowerBar.gameObject.SetActive(false);
        flamethrowerText.gameObject.SetActive(false);
    }
    */


}
