using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class PotatoTowerArcher : MonoBehaviour
{
    [Header("Tower Settings")]
    public float range = 100.0f;
    public float attackDelay = 1.0f;
    public float rotationSpeed = 5.0f;
    public float maxHealth = 100;
    // public float towerCost = 100f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float firePower;

    private float attackTimer;
    private float currentHealth;
    private List<SweatPotatoEnemy> enemiesInRange = new List<SweatPotatoEnemy>();
    private SweatPotatoEnemy targetEnemy;

    private void Start()
    {
        attackTimer = attackDelay;
        currentHealth = maxHealth;
        SphereCollider rangeCollider = gameObject.AddComponent<SphereCollider>();
        rangeCollider.radius = range;
        rangeCollider.isTrigger = true;
    }

    void Update()
    {
        enemiesInRange.RemoveAll(e => e == null);
        targetEnemy = FindClosestEnemy();

        if (targetEnemy != null)
        {
            RotateTowardsTarget();

            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
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

    private SweatPotatoEnemy FindClosestEnemy()
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

        return closestEnemy;
    }

    void FireProjectile()
    {
        GameObject arrow = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        arrow.GetComponent<Rigidbody>().AddForce(firePoint.transform.forward * firePower, ForceMode.Impulse );
        // TODO :  À CHANGER POUR METTRE LA POSITION DE L'ENNEMI LE PLUS PRÈS 
    }

    private void OnTriggerEnter(Collider other)
    {
        SweatPotatoEnemy enemy = other.GetComponent<SweatPotatoEnemy>();
        if (enemy != null && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);

            // Recalcule immédiatement la cible
            targetEnemy = FindClosestEnemy();

            // Tire instantanément
            FireProjectile();
            attackTimer = attackDelay;
        }
    }

    public void LoseLife(float damage)
    {
        Debug.Log($"TowerArcher touchée ! HP restants : {currentHealth}");
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
