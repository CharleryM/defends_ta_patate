using UnityEngine;

public class spawn : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform target; // L'objet vers lequel l'élément va se déplacer
    public float spawnRadius = 25f;
    public float spawnDelay = 2f;
    public float timer = 0f;

    void Start()
    {
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnDelay)
        {
            Spawn();
            timer = 0f;
        }
        
    }

    void Spawn()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y,
            transform.position.z + randomCircle.y
        );

        GameObject spawned = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        EnemyMouving mover = spawned.GetComponent<EnemyMouving>();
        if (mover != null)
        {
            mover.SetTarget(target);
        }
    }
}
