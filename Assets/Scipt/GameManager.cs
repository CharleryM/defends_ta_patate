using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float startingCurrency = 500f;
    [SerializeField] private int startingLives = 20;
    [SerializeField] private float waveDuration = 30f;
    [SerializeField] private float timeBetweenWaves = 10f;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private GameObject gameOverPanel;
    
    [Header("Enemy Settings")]
    [SerializeField] private EnemySpawner enemySpawner;

    private float currentCurrency;
    private int currentLives;
    private int currentWave = 0;
    private float waveTimer;
    private bool isWaveActive = false;
    private bool isGameOver = false;
    private Coroutine notificationCoroutine;

    public float CurrentCurrency => currentCurrency;
    public int CurrentLives => currentLives;
    public int CurrentWave => currentWave;
    public bool IsGameOver => isGameOver;

    private void Start()
    {
        // Initialize game state
        currentCurrency = startingCurrency;
        currentLives = startingLives;
        
        // Initialize UI
        UpdateUI();
        
        // Hide game over panel
        if (gameOverPanel)
            gameOverPanel.SetActive(false);
            
        // Start the first wave after a delay
        StartCoroutine(StartNextWaveAfterDelay(3f));
    }

    private void Update()
    {
        if (isGameOver)
            return;
            
        if (isWaveActive)
        {
            // Update wave timer
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0f)
            {
                // End the current wave
                EndWave();
            }
        }
        
        // Update timer UI
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText)
        {
            if (isWaveActive)
            {
                // Show remaining wave time
                timerText.text = $"Wave Time: {Mathf.CeilToInt(waveTimer)}s";
            }
            else
            {
                // Show time until next wave
                timerText.text = $"Next Wave: {Mathf.CeilToInt(waveTimer)}s";
            }
        }
    }
    
    private void UpdateUI()
    {
        if (currencyText)
            currencyText.text = $"Currency: {currentCurrency}";
            
        if (livesText)
            livesText.text = $"Lives: {currentLives}";
            
        if (waveText)
            waveText.text = $"Wave: {currentWave}";
    }

    public void DeductCurrency(float amount)
    {
        currentCurrency -= amount;
        UpdateUI();
    }

    public void AddCurrency(float amount)
    {
        currentCurrency += amount;
        UpdateUI();
    }

    public void LoseLife(int amount = 1)
    {
        currentLives -= amount;
        UpdateUI();
        
        // Check if game over
        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        
        // Show game over UI
        if (gameOverPanel)
            gameOverPanel.SetActive(true);
            
        // Stop enemy spawning
        if (enemySpawner)
            enemySpawner.StopSpawning();
            
        Debug.Log("Game Over!");
    }

    private void StartWave()
    {
        currentWave++;
        isWaveActive = true;
        waveTimer = waveDuration;
        
        UpdateUI();
        
        // Start spawning enemies for this wave
        if (enemySpawner)
            enemySpawner.StartWave(currentWave);
            
        ShowNotification($"Wave {currentWave} Started!");
    }

    private void EndWave()
    {
        isWaveActive = false;
        waveTimer = timeBetweenWaves;
        
        // Award currency for completing wave
        AddCurrency(currentWave * 100);
        
        ShowNotification($"Wave {currentWave} Completed! +{currentWave * 100} Currency");
        
        // Queue up the next wave
        StartCoroutine(StartNextWaveAfterDelay(timeBetweenWaves));
    }

    private IEnumerator StartNextWaveAfterDelay(float delay)
    {
        waveTimer = delay;
        yield return new WaitForSeconds(delay);
        if (!isGameOver)
            StartWave();
    }

    public void ShowNotification(string message, float duration = 3f)
    {
        if (notificationText)
        {
            // Stop any existing notification coroutine
            if (notificationCoroutine != null)
                StopCoroutine(notificationCoroutine);
                
            // Start new notification
            notificationCoroutine = StartCoroutine(ShowNotificationCoroutine(message, duration));
        }
    }

    private IEnumerator ShowNotificationCoroutine(string message, float duration)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(duration);
        
        notificationText.gameObject.SetActive(false);
        notificationCoroutine = null;
    }

    public void RestartGame()
    {
        // Reload the current scene to restart the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}