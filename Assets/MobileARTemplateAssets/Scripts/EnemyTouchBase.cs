using UnityEngine;

public class EnemyTouchBase : MonoBehaviour
{
    public int damage = 10;
    public float damageInterval = 1f;

    private bool isTouchingTower = false;
    private float damageTimer = 0f;
    private HealthPoints currentTarget;

    void Update()
    {
        
        if (isTouchingTower && currentTarget != null)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                currentTarget.TakeDamage(damage);
                damageTimer = 0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            Debug.Log("🧟 L'ennemi a touché la tour !");

            if (collision.gameObject.TryGetComponent<HealthPoints>(out var towerHealth))
            {
                currentTarget = towerHealth;
                towerHealth.TakeDamage(damage); 
                isTouchingTower = true;
                damageTimer = 0f;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tower"))
        {
            isTouchingTower = false;
            currentTarget = null;
            damageTimer = 0f;
        }
    }
}
