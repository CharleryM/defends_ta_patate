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

            // Tente de récupérer le script SweetPotatoEnemy
            SweetPotatoEnemy enemy = collision.gameObject.GetComponent<SweetPotatoEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageOnCollision);
            }
            else
            {
                Debug.LogWarning("⚠️ Aucun script SweetPotatoEnemy trouvé sur " + collision.gameObject.name);
            }
        }
    }
}