using UnityEngine;

public class SweatPotatoEnemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private int pointsValue = 10;

    private float currentHealth;
    private Transform baseTarget;
    private BasePotato gameManager;

    void Start()
    {
        currentHealth = maxHealth;
        gameObject.tag = "Enemy";

        GameObject baseObj = GameObject.FindGameObjectWithTag("BasePotato");
        if (baseObj != null)
        {
            baseTarget = baseObj.transform;
            gameManager = baseObj.GetComponent<BasePotato>();
        }

        if (baseTarget == null)
        {
            Debug.LogError("Aucun objet avec le tag 'BasePotato' trouvé !");
        }
        else
        {
            Debug.Log("Cible détectée : BasePotato");
        }
    }

    void Update()
    {
        if (baseTarget == null) return;

        MoveTowardsBase();
    }

    private void MoveTowardsBase()
    {
        Vector3 targetPosition = new Vector3(baseTarget.position.x, transform.position.y, baseTarget.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            ReachBase();
        }
    }

    private void ReachBase()
    {
        if (gameManager != null)
        {
            gameManager.LoseLife(20);
        }

        Debug.Log("L'ennemi a atteint la base !");
        Destroy(gameObject);
    }

    public void ApplyHealthMultiplier(float multiplier)
    {
        maxHealth *= multiplier;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        // Debug.Log($"[Enemy: {gameObject.name}] Touché - PV restants : {currentHealth}");

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

        Debug.Log("💀 Ennemi détruit !");
        Destroy(gameObject);
    }
}
