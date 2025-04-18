using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageOnCollision = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("💥 Boom ! Ennemi touché par " + gameObject.name);

            // Tente de récupérer le script SweatPotatoEnemy
            SweatPotatoEnemy enemy = collision.gameObject.GetComponent<SweatPotatoEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageOnCollision);
            }
            else
            {
                Debug.LogWarning("⚠️ Aucun script SweatPotatoEnemy trouvé sur " + collision.gameObject.name);
            }
        }
    }
}