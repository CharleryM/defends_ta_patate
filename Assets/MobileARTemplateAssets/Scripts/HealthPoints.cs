using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthPoints : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} a {currentHealth} PV");

        if (currentHealth <= 0)
            Die();
    }

    public void ApplyDamage(int amount, float delay = 0f)
    {
        if (delay > 0)
            StartCoroutine(DelayedDamage(amount, delay));
        else
            TakeDamage(amount);
    }

    private IEnumerator DelayedDamage(int amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        TakeDamage(amount);
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");
        Destroy(gameObject);
    }
}