using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 25f;
    public float lifetime = 2f;
    public GameObject impactEffect;
    
    private GameObject target;
    private Vector3 lastKnownPosition;
    
    void Start()
    {
        // Destroy the projectile after lifetime seconds
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        // If we lost our target, move towards last known position
        if (target == null)
        {
            transform.position = Vector3.MoveTowards(transform.position, lastKnownPosition, speed * Time.deltaTime);
            
            // Destroy if we've reached the last known position
            if (Vector3.Distance(transform.position, lastKnownPosition) < 0.1f)
            {
                DestroyProjectile();
            }
            return;
        }
        
        // Update the last known position
        lastKnownPosition = target.transform.position;
        
        // Move towards the target
        Vector3 direction = target.transform.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        
        // Rotate to face the direction we're moving
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Check if we hit the target
        float distanceThisFrame = speed * Time.deltaTime;
        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
        }
    }
    
    public void SetTarget(GameObject target)
    {
        this.target = target;
        lastKnownPosition = target.transform.position;
    }
    
    void HitTarget()
    {
        // Create impact effect
        if (impactEffect != null)
        {
            GameObject impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(impact, 2f); // Destroy effect after 2 seconds
        }
        
        // Apply damage to the enemy
        if (target != null)
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        
        // Destroy the projectile
        DestroyProjectile();
    }
    
    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if we collided with the target
        if (target != null && other.gameObject == target)
        {
            HitTarget();
        }
        // Or any enemy (in case we lost our target)
        else if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                DestroyProjectile();
            }
        }
    }
}