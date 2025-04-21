using UnityEngine;

public class StraightArrow : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    public float damage = 25f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            SweatPotatoEnemy enemy = other.GetComponent<SweatPotatoEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}