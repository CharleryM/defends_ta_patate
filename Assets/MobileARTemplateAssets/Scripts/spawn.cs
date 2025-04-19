using UnityEngine;

public class spawn : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform target; // L'objet vers lequel l'élément va se déplacer
    public float spawnRadius = 25f;

    void Start()
    {
    }

    void Update()
    {
        Spawn();
    }

    void Spawn()
    {
        // Position aléatoire autour du spawner
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y,
            transform.position.z + randomCircle.y
        );

        GameObject spawned = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // On passe la cible à l'objet qui va se déplacer
        EnemyMouving mover = spawned.GetComponent<EnemyMouving>();
        if (mover != null)
        {
            mover.SetTarget(target);
        }
    }
}
