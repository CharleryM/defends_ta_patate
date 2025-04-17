using UnityEngine;
using System.Collections;

public class SweatPotatoEnemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private int pointsValue = 10;

    [Header("Path Following")]
    [SerializeField] private Transform[] waypoints;

    private int currentWaypointIndex = 0;
    private float currentHealth;
    private BasePotato gameManager;

    void Start()
    {
        currentHealth = maxHealth;
        gameManager = FindObjectOfType<BasePotato>();
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 targetPosition = new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z);

        // Déplacement
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Rotation vers la direction de mouvement
        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
        }

        // Si on atteint le waypoint
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                ReachEnd();
            }
        }
    }
    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
    }

    public void ApplyHealthMultiplier(float multiplier)
    {
        maxHealth *= multiplier;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (gameManager != null)
        {
            gameManager.AddPoints(pointsValue);
        }

        Destroy(gameObject);
    }

    private void ReachEnd()
    {
        if (gameManager != null)
        {
            gameManager.LoseLife();
        }

        Destroy(gameObject);
    }
}
