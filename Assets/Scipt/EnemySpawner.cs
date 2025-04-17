using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject prefab;
        public float health;
        public float speed;
        public float reward;
        public int damage;
    }

    [Header("Spawning Settings")]
    [SerializeField] private List<EnemyType> enemyTypes = new List<EnemyType>();
    [SerializeField] private Transform target;
    [SerializeField] private float spawnHeight = 0.1f;
    [SerializeField] private float minSpawnDistance = 2f;
    [SerializeField] private float maxSpawnDistance = 5f;
    [SerializeField] private int baseEnemiesPerWave = 5;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ARPlaneManager planeManager;

    private Camera mainCamera;
    private List<ARPlane> validPlanes = new List<ARPlane>();
    private Coroutine spawnCoroutine;
    private int enemiesRemainingInWave;

    private void Start()
    {
        mainCamera = Camera.main;
        
        // Validate required components
        if (enemyTypes.Count == 0)
            Debug.LogError("No enemy types assigned!");
            
        if (target == null)
            Debug.LogError("Target transform not assigned!");
            
        if (gameManager == null)
            Debug.LogError("Game Manager not assigned!");
            
        if (planeManager == null)
            Debug.LogError("AR Plane Manager not assigned!");
    }

    public void StartWave(int waveNumber)
    {
        // Calculate number of enemies based on wave number
        enemiesRemainingInWave = baseEnemiesPerWave + (waveNumber * 2);
        
        // Start spawning
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        
        spawnCoroutine = StartCoroutine(SpawnEnemiesForWave());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnEnemiesForWave()
    {
        // Get all valid planes for spawning
        UpdateValidPlanes();
        
        while (enemiesRemainingInWave > 0 && !gameManager.IsGameOver)
        {
            SpawnEnemy();
            enemiesRemainingInWave--;
            
            yield return new WaitForSeconds(spawnInterval);
        }
        
        spawnCoroutine = null;
    }

    private void UpdateValidPlanes()
    {
        validPlanes.Clear();
        
        foreach (ARPlane plane in planeManager.trackables)
        {
            // Only use horizontal planes for spawning
            if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
            {
                validPlanes.Add(plane);
            }
        }
    }

    private void SpawnEnemy()
    {
        // Make sure we have valid planes to spawn on
        if (validPlanes.Count == 0)
        {
            Debug.LogWarning("No valid planes for spawning enemies!");
            return;
        }

        // Select a random plane to spawn on
        ARPlane spawnPlane = validPlanes[Random.Range(0, validPlanes.Count)];
        
        // Select a random position on the plane
        Vector3 spawnPosition = GetRandomPositionOnPlane(spawnPlane);
        spawnPosition.y += spawnHeight; // Raise slightly above the plane
        
        // Select an enemy type based on the current wave (higher waves introduce stronger enemies)
        int maxEnemyIndex = Mathf.Min(gameManager.CurrentWave / 2, enemyTypes.Count - 1);
        EnemyType enemyType = enemyTypes[Random.Range(0, maxEnemyIndex + 1)];
        
        // Create the enemy
        GameObject enemyObject = Instantiate(enemyType.prefab, spawnPosition, Quaternion.identity);
        
        // Setup enemy components
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (enemy == null)
            enemy = enemyObject.AddComponent<Enemy>();
            
        // Configure enemy
        enemy.Setup(target, enemyType.health, enemyType.speed, enemyType.reward, enemyType.damage, gameManager);
    }

    private Vector3 GetRandomPositionOnPlane(ARPlane plane)
    {
        // Get a random point on the plane
        Bounds planeBounds = plane.GetComponent<MeshRenderer>().bounds;
        Vector3 randomPoint = new Vector3(
            Random.Range(planeBounds.min.x, planeBounds.max.x),
            planeBounds.center.y,
            Random.Range(planeBounds.min.z, planeBounds.max.z)
        );
        
        // Ensure minimum distance from camera/player
        Vector3 cameraPosition = mainCamera.transform.position;
        float distanceToCamera = Vector3.Distance(randomPoint, cameraPosition);
        
        // If too close, push point away from camera
        if (distanceToCamera < minSpawnDistance)
        {
            Vector3 directionFromCamera = (randomPoint - cameraPosition).normalized;
            randomPoint = cameraPosition + directionFromCamera * minSpawnDistance;
        }
        // If too far, pull point closer to camera
        else if (distanceToCamera > maxSpawnDistance)
        {
            Vector3 directionFromCamera = (randomPoint - cameraPosition).normalized;
            randomPoint = cameraPosition + directionFromCamera * maxSpawnDistance;
        }
        
        return randomPoint;
    }
}