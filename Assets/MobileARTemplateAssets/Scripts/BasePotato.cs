using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class BasePotato : MonoBehaviour
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

        currentLives = startingLives;
        currentMoney = startingMoney;
        UpdateUI();

        if (startGameButton != null)
        {
            startGameButton.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameStarted) return;

        // Plane selection
        if (!placementMode && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                ARPlane plane = planeManager.GetPlane(hits[0].trackableId);

                if (plane != null)
                {
                    selectedPlane = plane.gameObject;
                    Vector3 hitPosition = hits[0].pose.position;

                    pathManager.CreatePath(selectedPlane.transform, hitPosition);
                    startGameButton?.SetActive(true);

                    placementMode = true;
                    planeManager.enabled = false;
                    uiManager?.ShowGameUI();
                }
            }
        }

        // Tower placement
        if (placementMode && !gameStarted && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (currentMoney >= 100)
            {
                Touch touch = Input.GetTouch(0);
                List<ARRaycastHit> hits = new List<ARRaycastHit>();

                if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
                {
                    ARPlane plane = planeManager.GetPlane(hits[0].trackableId);

                    if (plane != null && plane.gameObject == selectedPlane)
                    {
                        Vector3 hitPosition = hits[0].pose.position;
                        GameObject tower = Instantiate(towerPrefab, hitPosition, Quaternion.identity);
                        towers.Add(tower);

                        PotatoTowerArcher towerScript = tower.GetComponent<PotatoTowerArcher>();
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
        startGameButton?.SetActive(false);
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        for (int wave = 1; wave <= wavesCount; wave++)
        {
            currentWave = wave;
            UpdateUI();

            yield return StartCoroutine(SpawnWave(wave));
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private IEnumerator SpawnWave(int waveNumber)
    {
        int enemiesToSpawn = enemiesPerWave + (waveNumber - 1) * 2;

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

        Vector3 startPosition = pathManager.GetWaypoints()[0].position;
        GameObject enemy = Instantiate(enemyPrefab, startPosition, Quaternion.identity);

        SweatPotatoEnemy enemyScript = enemy.GetComponent<SweatPotatoEnemy>();
        if (enemyScript != null)
        {
            enemyScript.SetWaypoints(pathManager.GetWaypoints());
            float healthMultiplier = 1f + (waveNumber - 1) * 0.2f;
            enemyScript.ApplyHealthMultiplier(healthMultiplier);
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
        gameStarted = false;
        uiManager?.ShowGameOverUI();
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
        foreach (GameObject tower in towers)
        {
            Destroy(tower);
        }
        towers.Clear();

        SweatPotatoEnemy[] enemies = FindObjectsOfType<SweatPotatoEnemy>();
        foreach (SweatPotatoEnemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        currentLives = startingLives;
        currentMoney = startingMoney;
        currentWave = 0;
        gameStarted = false;
        placementMode = false;

        pathManager.ClearPath();
        UpdateUI();

        planeManager.enabled = true;
        startGameButton?.SetActive(false);
        uiManager?.ShowPlaneDetectionUI();
    }
}
