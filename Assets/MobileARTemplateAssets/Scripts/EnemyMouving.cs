using UnityEngine;

public class EnemyMouving : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
    }
}