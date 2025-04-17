using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : MonoBehaviour
{
    public float range = 3.0f;            // Attack range
    public float attackDelay = 1.0f;      // Time between attacks
    public float rotationSpeed = 5.0f;    // How fast the tower rotates to face enemies
    public GameObject projectilePrefab;   // The projectile the tower shoots
    public Transform firePoint;           // Where projectiles spawn
    
    private float attackTimer;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private Enemy targetEnemy;
    
    public float towerCost = 100f;        // Cost to build this tower
    
    void Start()
    {
        // Initialize attack timer
        attackTimer = attackDelay;
        
        // Create a sphere collider for detecting enemies
        SphereCollider rangeCollider = gameObject.AddComponent<SphereCollider>();
        rangeCollider.radius = range;
        rangeCollider.isTrigger = true;
    }

    void Update()
    {
        // Remove any null enemies (destroyed ones)
        enemiesInRange.RemoveAll(e => e == null);
        
        // Find the closest enemy as the target
        FindClosestEnemy();
        
        // If we have a target, rotate towards it and attack
        if (targetEnemy != null)
        {
            // Rotate to face the target
            Vector3 direction = targetEnemy.transform.position - transform.position;
            direction.y = 0; // Keep rotation only on Y axis
            
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }
            
            // Attack timer countdown
            attackTimer -= Time.deltaTime;
            
            // When timer reaches zero, fire a projectile
            if (attackTimer <= 0)
            {
                FireProjectile();
                attackTimer = attackDelay; // Reset timer
            }
        }
    }
    
    void FindClosestEnemy()
    {
        float closestDistance = float.MaxValue;
        Enemy closestEnemy = null;
        
        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        
        targetEnemy = closestEnemy;
    }
    
    void FireProjectile()
    {
        if (projectilePrefab != null && targetEnemy != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            
            if (projectileScript != null)
            {
                projectileScript.SetTarget(targetEnemy.gameObject);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Add enemy to the list when it enters range
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Remove enemy from the list when it leaves range
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
            
            // If this was our target, clear the target
            if (enemy == targetEnemy)
            {
                targetEnemy = null;
            }
        }
    }
    
    // Draw range gizmo in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}