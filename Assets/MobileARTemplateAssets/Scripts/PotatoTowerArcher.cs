using UnityEngine;
using System.Collections.Generic;

public class PotatoTowerArcher : MonoBehaviour
{
    [Header("Tower Settings")]
    public float range = 3.0f;
    public float attackDelay = 1.0f;
    public float rotationSpeed = 5.0f;
    public float towerCost = 100f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float attackTimer;
    private List<SweatPotatoEnemy> enemiesInRange = new List<SweatPotatoEnemy>();
    private SweatPotatoEnemy targetEnemy;

    void Start()
    {
        attackTimer = attackDelay;

        SphereCollider rangeCollider = gameObject.AddComponent<SphereCollider>();
        rangeCollider.radius = range;
        rangeCollider.isTrigger = true;
    }

    void Update()
    {
        enemiesInRange.RemoveAll(e => e == null);
        FindClosestEnemy();

        if (targetEnemy != null)
        {
            RotateTowardsTarget();

            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                FireProjectile();
                attackTimer = attackDelay;
            }
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = targetEnemy.transform.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void FindClosestEnemy()
    {
        float closestDistance = float.MaxValue;
        SweatPotatoEnemy closestEnemy = null;

        foreach (SweatPotatoEnemy enemy in enemiesInRange)
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

    private void FireProjectile()
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
        SweatPotatoEnemy enemy = other.GetComponent<SweatPotatoEnemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SweatPotatoEnemy enemy = other.GetComponent<SweatPotatoEnemy>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
            if (enemy == targetEnemy)
            {
                targetEnemy = null;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
