using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    // UI elements
    [SerializeField] TextMeshProUGUI titleScreen;
    [SerializeField] Button startButton;
    [SerializeField] Button stopButton;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI gameOver1;
    [SerializeField] TextMeshProUGUI gameOver2;
    [SerializeField] RawImage gameOverTint;
    [SerializeField] TextMeshProUGUI timerText;
    public int enemiesKilledDuringPlay;

    // Variables
    private PlayerController playerControllerScript;
    private SpawnManager spawnManagerScript;
    private float secondsCount;
    private int minuteCount;

    // Start is called before the first frame update
    void Start()
    {
        // Finding scripts
        playerControllerScript = GameObject.Find("Enemy Target Point").GetComponent<PlayerController>();
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        //adding listener to button
        startButton.onClick.AddListener(StartGame);

        //adding listener to button
        stopButton.onClick.AddListener(StopGame);

    }

    // Update is called once per frame
    void Update()
    {

        //KillWaveDebug();

        // Activating UI gameplay components
        if (spawnManagerScript.gameActive)
        {
            UpdateTimerUI();
            HealthManager(playerControllerScript.healthCount);
            gameOver1.gameObject.SetActive(false);
            gameOver2.gameObject.SetActive(false);
            spawnManagerScript.easyMode.gameObject.SetActive(false);
            spawnManagerScript.mediumMode.gameObject.SetActive(false);
            spawnManagerScript.hardMode.gameObject.SetActive(false);
        }
    }

    void ChooseDifficultyUI()
    {
        spawnManagerScript.easyMode.gameObject.SetActive(true);
        spawnManagerScript.mediumMode.gameObject.SetActive(true);
        spawnManagerScript.hardMode.gameObject.SetActive(true);
    }

    // Method to start game...
    void StartGame()
    {
        //debug test
        //Debug.Log("Button Clicked!");

        // Telling spawn manager that game is active
        spawnManagerScript.gameActive = true;

        healthText.gameObject.SetActive(true);

        /*
        // Additional UI and player state setup code (e.g., resetting health, UI elements, etc.)
        
        //playerControllerScript.grave.gameObject.SetActive(false);
        //playerAnim.SetBool("isDead", false);
        minuteCount = 0;
        secondsCount = 0;

        waveText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);

        */
        //UI
        startButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);

    }

    void StopGame()
    {
        //debug test
        //Debug.Log("Button Clicked!");

        //UI
        stopButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);

        spawnManagerScript.gameActive = false;
        ResetForNextPlay();
        //spawnManagerScript.DeactivateAllEnemies();
    }

    // Method to call the game over screen and reset elements for the next play
    void GameOverScreen()
    {
        // Stopping spawning by making game inactive
        spawnManagerScript.gameActive = false;

        // Update game over UI

        healthText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);

        gameOver2.text = "You survived for " + minuteCount + " mins " + (int)secondsCount + " secs" +
            "\n" + "and killed " + enemiesKilledDuringPlay + " enemies";
            
        gameOver1.gameObject.SetActive(true);
        gameOver2.gameObject.SetActive(true);
        //gameOverTint.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);

        // Brief pause before start button appears
        //StartCoroutine(RestartButtonPause());
        
        // Return all enemies to the pool instead of destroying them
        foreach (GameObject enemy in spawnManagerScript.activeEnemies)
        {
            spawnManagerScript.DeactivateEnemy(enemy);
        }

        // Destroying all remaining power-ups at the end of the game
        GameObject[] powerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject powerUp in powerUps)
        {
            Destroy(powerUp);
        }
    }

    // Brief pause on Game Over before the restart button appears
    IEnumerator RestartButtonPause()
    {
        yield return new WaitForSeconds(3.5f);
        startButton.gameObject.SetActive(true);
    }

    // Reset game elements for the next play
    void ResetForNextPlay()
    {
        playerControllerScript.ResetHealth();
        spawnManagerScript.ResetNextWave();
    }

    // Method to manage a UI timer
    void UpdateTimerUI()
    {
        // Set timer UI
        secondsCount += Time.deltaTime;

        // Formatting string for UI
        timerText.text = string.Format("{0:00}:{1:00}", minuteCount, secondsCount);

        // Count to 60 seconds, then add minute to counter and reset seconds
        if (secondsCount >= 60)
        {
            minuteCount++;
            secondsCount = 0;
        }
    }

    //method to keep health text up to date
    public void HealthManager(int pHealthNo)
    {
        if (playerControllerScript.healthCount > 0)
        {
            healthText.text = "Health: " + pHealthNo + "/3";
        }
        else
        {
            GameOverScreen();
            ResetForNextPlay();
        }
    }



    // Debugging method to progress through waves (deactivating enemies instead of destroying them)
    void KillWaveDebug()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (GameObject enemy in spawnManagerScript.activeEnemies)
            {
                spawnManagerScript.DeactivateEnemy(enemy);
            }
        }
    }

}
