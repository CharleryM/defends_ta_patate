using UnityEngine;

public class moving : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;
        
// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );}
}
