using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform target;
    private float health;
    private float speed;
    private float reward;
    private int damage;
    private GameManager gameManager;
    
    private bool isActive = true;

    public void Setup(Transform targetTransform, float enemyHealth, float enemySpeed, float enemyReward, int enemyDamage, GameManager gm)
    {
        target = targetTransform;
        health = enemyHealth;
        speed = enemySpeed;
        reward = enemyReward;
        damage = enemyDamage;
        gameManager = gm;
    }

    private void Update()
    {
        if (!isActive || target == null || gameManager.IsGameOver)
            return;
            
        // Move towards target
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
        
        // Look at target (on Y axis only to keep enemy upright)
        Vector3 targetDirection = target.position - transform.position;
        targetDirection.y = 0;
        if (targetDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(targetDirection);
        }
        
        // Check if we've reached the target
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            ReachTarget();
        }
    }

    private void ReachTarget()
    {
        // Damage player lives
        if (gameManager != null)
        {
            gameManager.LoseLife(damage);
        }
        
        // Destroy self
        Destroy(gameObject);
    }

    public void TakeDamage(float damageAmount)
    {
        if (!isActive)
            return;
            
        health -= damageAmount;
        
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isActive = false;
        
        // Award currency for kill
        if (gameManager != null)
        {
            gameManager.AddCurrency(reward);
        }
        
        // Optional: Play death animation/effect
        
        // Destroy the game object
        Destroy(gameObject, 0.1f);
    }
}