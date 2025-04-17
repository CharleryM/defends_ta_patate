using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float maxLifetime = 2f;
    [SerializeField] private GameObject impactEffect;
    
    private Transform target;
    private float damage;
    private bool hasHit = false;

    public void Initialize(Transform targetTransform, float damageAmount)
    {
        target = targetTransform;
        damage = damageAmount;
        
        // Destroy after max lifetime to prevent projectiles from flying forever
        Destroy(gameObject, maxLifetime);
    }

    private void Update()
    {
        if (hasHit || target == null)
            return;
            
        // Move toward target
        Vector3 direction = target.position - transform.position;
        float distanceToTarget = direction.magnitude;
        
        // Normalize and apply speed
        direction = direction.normalized;
        float moveDistance = speed * Time.deltaTime;
        
        // Move the projectile
        transform.Translate(direction * moveDistance, Space.World);
        
        // Look in the direction of movement
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Check if we've reached the target
        if (distanceToTarget <= moveDistance)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        hasHit = true;
        
        // Apply damage to enemy
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        
        // Spawn impact effect if set
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, transform.rotation);
        }
        
        // Destroy the projectile
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Alternative hit detection using colliders
        if (hasHit)
            return;
            
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Set target if we don't have one (for non-homing projectiles)
            if (target == null)
                target = enemy.transform;
                
            HitTarget();
        }
    }
}