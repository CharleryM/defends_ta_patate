using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    public Transform[] waypoints;
    public GameObject waypointPrefab;
    public float pathHeight = 0.05f; // Height of waypoints above the AR plane
    
    public int waypointCount = 5; // Number of waypoints to create
    public float pathLength = 5f; // Approximate length of the path
    
    private List<Transform> generatedWaypoints = new List<Transform>();
    
    // Create a path on the AR plane
    public void CreatePath(Transform plane, Vector3 startPoint)
    {
        // Clear any existing waypoints
        ClearPath();
        
        // Calculate path parameters
        float pathWidth = pathLength * 0.6f;
        Vector3 currentPosition = startPoint;
        
        // Create start point
        AddWaypoint(currentPosition + Vector3.up * pathHeight);
        
        // Create middle waypoints with some randomness for a winding path
        Vector3 direction = Vector3.forward;
        float segment = pathLength / (waypointCount - 1);
        
        for (int i = 1; i < waypointCount - 1; i++)
        {
            // Add some randomness to the direction
            float angle = Random.Range(-45f, 45f);
            direction = Quaternion.Euler(0, angle, 0) * direction;
            
            // Make sure we don't go too far off to the sides
            Vector3 toCenter = startPoint - currentPosition;
            toCenter.y = 0;
            if (toCenter.magnitude > pathWidth * 0.5f)
            {
                // Steer back toward center
                direction = (direction + toCenter.normalized).normalized;
            }
            
            // Calculate new position
            currentPosition += direction * segment;
            
            // Keep it on the plane
            currentPosition.y = plane.position.y;
            
            // Add the waypoint
            AddWaypoint(currentPosition + Vector3.up * pathHeight);
        }
        
        // Create end point (typically near the edge of the plane)
        Vector3 endDirection = (currentPosition - startPoint).normalized;
        if (endDirection == Vector3.zero) endDirection = Vector3.forward;
        
        Vector3 endPoint = currentPosition + endDirection * segment;
        endPoint.y = plane.position.y;
        
        AddWaypoint(endPoint + Vector3.up * pathHeight);
        
        // Convert to array for easier use by enemies
        waypoints = generatedWaypoints.ToArray();
    }
    
    private void AddWaypoint(Vector3 position)
    {
        GameObject waypoint = Instantiate(waypointPrefab, position, Quaternion.identity);
        waypoint.transform.SetParent(transform);
        generatedWaypoints.Add(waypoint.transform);
    }
    
    // Clear all waypoints
    public void ClearPath()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        generatedWaypoints.Clear();
    }
    
    // Get the array of waypoints to be used by enemies
    public Transform[] GetWaypoints()
    {
        return waypoints;
    }
}