using UnityEngine;

public class healthPoints : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    
    public float damageInterval = 1f;
    private float timer = 0f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= damageInterval)
        {
            TakeDamage(10);
            timer = 0f;
        }
    }

    public void TakeDamage(int amount = 10)
    {
        currentHealth -= amount;
        Debug.Log("L'ennemi a " + currentHealth + " PV");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        Debug.Log("L'ennemi est mort !");
        Destroy(gameObject);
    }
}
