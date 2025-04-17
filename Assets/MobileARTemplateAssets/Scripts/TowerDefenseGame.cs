using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class TowerDefenseGame : MonoBehaviour
{
    [Header("AR Components")]
    public ARPlaneManager planeManager;
    public ARRaycastManager raycastManager;
    
    [Header("Game Objects")]
    public GameObject towerPrefab;
    public GameObject enemyPrefab;
    public GameObject pathMarkerPrefab;
    public GameObject startGameButton;
    
    [Header("Game Settings")]
    public int startingLives = 5;
    public int startingMoney = 200;
    public int wavesCount = 5;
    public float timeBetweenWaves = 30f;
    public int enemiesPerWave = 5;
    public float timeBetweenEnemies = 2f;
    
    [Header("UI Components")]
    public TowerDefenseUI uiManager;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI waveText;
    
    private int currentLives;
    private int currentMoney;
    private int currentWave = 0;
    private bool gameStarted = false;
    private bool placementMode = false;
    private PathManager pathManager;
    private GameObject selectedPlane;
    private List<GameObject> towers = new List<GameObject>();
    
    private void Start()
    {
        pathManager = GetComponent<PathManager>();
        if (pathManager == null)
        {
            pathManager = gameObject.AddComponent<PathManager>();
        }
        
        // Initialize game state
        currentLives = startingLives;
        currentMoney = startingMoney;
        
        // Update UI
        UpdateUI();
        
        // Hide start button until a plane is selected
        if (startGameButton != null)
        {
            startGameButton.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (gameStarted) return;
        
        // Handle plane selection
        if (!placementMode && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                ARPlane plane = planeManager.GetPlane(hits[0].trackableId);
                
                if (plane != null)
                {
                    // Select this plane
                    selectedPlane = plane.gameObject;
                    
                    // Create a path on this plane
                    Vector3 hitPosition = hits[0].pose.position;
                    pathManager.CreatePath(selectedPlane.transform, hitPosition);
                    
                    // Show start button
                    if (startGameButton != null)
                    {
                        startGameButton.SetActive(true);
                    }
                    
                    // Enter placement mode
                    placementMode = true;
                    
                    // Stop tracking new planes
                    planeManager.enabled = false;
                    
                    // Notify UI
                    if (uiManager != null)
                    {
                        uiManager.ShowGameUI();
                    }
                }
            }
        }
        
        // Handle tower placement in placement mode
        if (placementMode && !gameStarted && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (currentMoney >= 100) // Check if player can afford tower
            {
                Touch touch = Input.GetTouch(0);
                
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    ARPlane plane = planeManager.GetPlane(hits[0].trackableId);
                    
                    if (plane != null && plane.gameObject == selectedPlane)
                    {
                        // Place tower at hit position
                        Vector3 hitPosition = hits[0].pose.position;
                        GameObject tower = Instantiate(towerPrefab, hitPosition, Quaternion.identity);
                        
                        // Add to our list of towers
                        towers.Add(tower);
                        
                        // Deduct cost
                        Tower towerScript = tower.GetComponent<Tower>();
                        if (towerScript != null)
                        {
                            currentMoney -= (int)towerScript.towerCost;
                            UpdateUI();
                        }
                    }
                }
            }
        }
    }
    
    public void StartGame()
    {
        if (!placementMode) return;
        
        gameStarted = true;
        
        // Hide start button
        if (startGameButton != null)
        {
            startGameButton.SetActive(false);
        }
        
        // Start spawning waves
        StartCoroutine(SpawnWaves());
    }
    
    private IEnumerator SpawnWaves()
    {
        for (int wave = 1; wave <= wavesCount; wave++)
        {
            currentWave = wave;
            UpdateUI();
            
            yield return StartCoroutine(SpawnWave(wave));
            
            // Wait between waves
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
    
    private IEnumerator SpawnWave(int waveNumber)
    {
        int enemiesToSpawn = enemiesPerWave + (waveNumber - 1) * 2; // More enemies in later waves
        
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy(waveNumber);
            yield return new WaitForSeconds(timeBetweenEnemies);
        }
    }
    
    private void SpawnEnemy(int waveNumber)
    {
        if (pathManager.GetWaypoints().Length == 0)
            return;
        
        // Spawn at first waypoint
        Vector3 startPosition = pathManager.GetWaypoints()[0].position;
        GameObject enemy = Instantiate(enemyPrefab, startPosition, Quaternion.identity);
        
        // Set up enemy properties
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.waypoints = pathManager.GetWaypoints();
            
            // Make enemies stronger in later waves
            float healthMultiplier = 1f + (waveNumber - 1) * 0.2f;
            enemyScript.health *= healthMultiplier;
        }
    }
    
    public void AddPoints(int points)
    {
        currentMoney += points;
        UpdateUI();
    }
    
    public void LoseLife()
    {
        currentLives--;
        UpdateUI();
        
        if (currentLives <= 0)
        {
            GameOver();
        }
    }
    
    private void GameOver()
    {
        // Stop game and show game over UI
        gameStarted = false;
        
        if (uiManager != null)
        {
            uiManager.ShowGameOverUI();
        }
    }
    
    private void UpdateUI()
    {
        if (livesText != null)
            livesText.text = "Lives: " + currentLives;
        
        if (moneyText != null)
            moneyText.text = "Money: " + currentMoney;
        
        if (waveText != null)
            waveText.text = "Wave: " + currentWave + "/" + wavesCount;
    }
    
    public void RestartGame()
    {
        // Clean up existing objects
        foreach (GameObject tower in towers)
        {
            Destroy(tower);
        }
        towers.Clear();
        
        // Destroy all enemies
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        
        // Reset game state
        currentLives = startingLives;
        currentMoney = startingMoney;
        currentWave = 0;
        gameStarted = false;
        placementMode = false;
        
        // Clear path
        pathManager.ClearPath();
        
        // Update UI
        UpdateUI();
        
        // Re-enable plane tracking
        planeManager.enabled = true;
        
        // Hide start button
        if (startGameButton != null)
        {
            startGameButton.SetActive(false);
        }
        
        // Notify UI
        if (uiManager != null)
        {
            uiManager.ShowPlaneDetectionUI();
        }
    }
}