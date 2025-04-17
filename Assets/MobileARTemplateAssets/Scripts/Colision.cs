using UnityEngine;

public class Colision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
     {
         if (collision.gameObject.CompareTag("Enemy"))
         {
             Debug.Log("Boom ! Ennemi touché");
             collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(10);
         }
     }
}
