using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // UI elements
    [SerializeField] TextMeshProUGUI titleScreen;
    [SerializeField] Button startButton;
    [SerializeField] Button stopButton;
    [SerializeField] Button easyMode;
    [SerializeField] Button mediumMode;
    [SerializeField] Button hardMode;
    [SerializeField] Button swapGunHandButton;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI gameOver1;
    [SerializeField] TextMeshProUGUI gameOver2;
    [SerializeField] TextMeshProUGUI timerText;

    //UI colours
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color defaultColor = Color.white;

    public int enemiesKilledDuringPlay;

    // Game difficulty settings
    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty gameDifficulty;

    // Variables
    private PlayerController playerControllerScript;
    private PostProcessingEffects postProcessingEffectsScript;
    private SpawnManager spawnManagerScript;
    private float secondsCount;
    private int minuteCount;

    // Start is called before the first frame update
    void Start()
    {
        // Finding scripts
        playerControllerScript = GameObject.Find("Enemy Target Point").GetComponent<PlayerController>();
        spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        postProcessingEffectsScript = GameObject.Find("Post Processing Controller").GetComponent<PostProcessingEffects>();

        // Adding listeners to buttons
        startButton.onClick.AddListener(DisplayDifficultyOptions);
        stopButton.onClick.AddListener(StopGame);
        easyMode.onClick.AddListener(() => SelectDifficulty(Difficulty.Easy));
        mediumMode.onClick.AddListener(() => SelectDifficulty(Difficulty.Medium));
        hardMode.onClick.AddListener(() => SelectDifficulty(Difficulty.Hard));

        // Initial UI setup
        InitializeUI();
    }

    // Update is called once per frame
    void Update()
    {
        // Activating UI gameplay components
        if (spawnManagerScript.gameActive)
        {
            postProcessingEffectsScript.OnGameRestart();
            UpdateTimerUI();
            HealthManager(playerControllerScript.healthCount);
            swapGunHandButton.gameObject.SetActive(true);
        }
    }

    void InitializeUI()
    {
        startButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
        swapGunHandButton.gameObject.SetActive(true);
        healthText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        postProcessingEffectsScript.OnPlayerDeath();
        HideDifficultyUI();
    }

    void DisplayDifficultyOptions()
    {
        // Hide the play button and show difficulty options
        startButton.gameObject.SetActive(false);
        swapGunHandButton.gameObject.SetActive(false);
        easyMode.gameObject.SetActive(true);
        mediumMode.gameObject.SetActive(true);
        hardMode.gameObject.SetActive(true);

    }

    void SelectDifficulty(Difficulty difficulty)
    {
        // Set the selected difficulty
        SetGameDifficulty(difficulty);

        // Hide difficulty buttons and start the game
        HideDifficultyUI();
        StartGame();
    }

    void StartGame()
    {
        //reset enemy kill count
        enemiesKilledDuringPlay = 0;

        gameOver1.gameObject.SetActive(false);
        gameOver2.gameObject.SetActive(false);

        // Set the game as active
        spawnManagerScript.gameActive = true;

        // Show gameplay UI
        timerText.gameObject.SetActive(true);
        healthText.gameObject.SetActive(true);
        swapGunHandButton.gameObject.SetActive(false);

        // Initialize gameplay variables
        minuteCount = 0;
        secondsCount = 0;

        // Show stop button
        stopButton.gameObject.SetActive(true);

    }

    //When Stop button is pressed:
    void StopGame()
    {
        spawnManagerScript.gameActive = false;
        ResetForNextPlay();
        stopButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
    }

    void SetGameDifficulty(Difficulty difficulty)
    {
        gameDifficulty = difficulty;

        // Update spawn manager settings
        switch (difficulty)
        {
            case Difficulty.Easy:
                spawnManagerScript.SetDifficultySettings(3f, 7f);
                break;
            case Difficulty.Medium:
                spawnManagerScript.SetDifficultySettings(2f, 5f);
                break;
            case Difficulty.Hard:
                spawnManagerScript.SetDifficultySettings(1.75f, 4f);
                break;
        }

        UpdateButtonColors();
    }

    // Highlight the currently selected difficulty button
    void UpdateButtonColors()
    {
        easyMode.image.color = (gameDifficulty == Difficulty.Easy) ? selectedColor : defaultColor;
        mediumMode.image.color = (gameDifficulty == Difficulty.Medium) ? selectedColor : defaultColor;
        hardMode.image.color = (gameDifficulty == Difficulty.Hard) ? selectedColor : defaultColor;
    }

    void HideDifficultyUI()
    {
        easyMode.gameObject.SetActive(false);
        mediumMode.gameObject.SetActive(false);
        hardMode.gameObject.SetActive(false);
    }

    // Game over logic
    void GameOverScreen()
    {
        spawnManagerScript.gameActive = false;
        postProcessingEffectsScript.OnPlayerDeath();

        // Update game over UI
        healthText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);

        gameOver2.text = "You survived for " + minuteCount + " mins " + (int)secondsCount + " secs" +
            "\n" + "and killed " + enemiesKilledDuringPlay + " enemies";

        gameOver1.gameObject.SetActive(true);
        gameOver2.gameObject.SetActive(true);

        stopButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);

        ResetForNextPlay();
    }

    // Reset counters for next run
    void ResetForNextPlay()
    {
        // Reset player health
        playerControllerScript.ResetHealth();

        // Reset UI and gameplay variables
        minuteCount = 0;
        secondsCount = 0;
        enemiesKilledDuringPlay = 0;

        // Ensure game is inactive until restarted
        spawnManagerScript.gameActive = false;

        // Reinitialize UI state
        InitializeUI();
    }

    void UpdateTimerUI()
    {
        secondsCount += Time.deltaTime;

        timerText.text = string.Format("{0:00}:{1:00}", minuteCount, secondsCount);

        if (secondsCount >= 60)
        {
            minuteCount++;
            secondsCount = 0;
        }
    }

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
}
