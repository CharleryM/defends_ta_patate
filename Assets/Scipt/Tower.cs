using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackSpeed = 1f; // Attacks per second
    [SerializeField] private LayerMask enemyLayer;
    
    [Header("Visual Effects")]
    [SerializeField] private Transform turretHead;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ParticleSystem muzzleFlash;
    
    private float attackCooldown;
    private Enemy currentTarget;

    private void Start()
    {
        attackCooldown = 0f;
        
        // If no enemy layer is set, assume default
        if (enemyLayer == 0)
            enemyLayer = LayerMask.GetMask("Default");
    }

    private void Update()
    {
        // Manage attack cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        
        // Find and attack targets
        if (currentTarget == null || !IsTargetInRange(currentTarget.transform))
        {
            // Find a new target
            FindTarget();
        }
        else
        {
            // Rotate turret to face target if we have one
            if (turretHead != null && currentTarget != null)
            {
                // Only rotate on Y axis to keep turret upright
                Vector3 targetDirection = currentTarget.transform.position - turretHead.position;
                targetDirection.y = 0;
                
                if (targetDirection != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
                    turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRotation, 5f * Time.deltaTime);
                }
            }
            
            // Attack if cooldown is ready
            if (attackCooldown <= 0)
            {
                Attack();
            }
        }
    }

    private void FindTarget()
    {
        // Find all enemies in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        
        // Find closest enemy
        float closestDistance = float.MaxValue;
        Enemy closestEnemy = null;
        
        foreach (Collider col in hitColliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        
        // Set as current target
        currentTarget = closestEnemy;
    }

    private bool IsTargetInRange(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) <= attackRange;
    }

    private void Attack()
    {
        // Reset cooldown
        attackCooldown = 1f / attackSpeed;
        
        // Apply damage to enemy
        currentTarget.TakeDamage(attackDamage);
        
        // Visual effects
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        
        // Create projectile if we have one
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            
            if (projectileScript != null)
            {
                projectileScript.Initialize(currentTarget.transform, attackDamage);
            }
            else
            {
                // Simple projectile without script - just make it move forward
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = firePoint.forward * 20f;
                }
                
                // Destroy after 2 seconds
                Destroy(projectile, 2f);
            }
        }
    }

    // Visualize the attack range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}