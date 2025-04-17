using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float speed = 1.0f;
    public float health = 100.0f;
    public Transform[] waypoints;
    public int waypointIndex = 0;
    public GameObject healthBarPrefab;
    private GameObject healthBar;
    public int pointsValue = 10;
    
    private TowerDefenseGame gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<TowerDefenseGame>();
        
        // Create health bar
        if (healthBarPrefab != null)
        {
            healthBar = Instantiate(healthBarPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            healthBar.transform.SetParent(transform);
            healthBar.GetComponent<HealthBar>().SetMaxHealth(health);
        }
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        // Move towards the next waypoint
        Transform targetWaypoint = waypoints[waypointIndex];
        Vector3 targetPosition = new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Rotate to face the movement direction
        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
        }

        // If we reached the waypoint, move to the next one
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            waypointIndex++;
            
            // If we reached the end of the path
            if (waypointIndex >= waypoints.Length)
            {
                ReachedEnd();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if (healthBar != null)
        {
            healthBar.GetComponent<HealthBar>().SetHealth(health);
        }
        
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (gameManager != null)
        {
            gameManager.AddPoints(pointsValue);
        }
        
        Destroy(gameObject);
    }
    
    void ReachedEnd()
    {
        if (gameManager != null)
        {
            gameManager.LoseLife();
        }
        
        Destroy(gameObject);
    }
}