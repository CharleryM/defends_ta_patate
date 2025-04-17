using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private float towerCost = 100f;
    [SerializeField] private GameManager gameManager;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        
        // Validate required components
        if (towerPrefab == null)
            Debug.LogError("Tower prefab not assigned!");
            
        if (planeManager == null)
            Debug.LogError("AR Plane Manager not assigned!");
            
        if (gameManager == null)
            Debug.LogError("Game Manager not assigned!");
    }

    public void PlaceTower(Vector3 position, Quaternion rotation)
    {
        // Check if player has enough currency
        if (gameManager && gameManager.CurrentCurrency >= towerCost)
        {
            // Deduct tower cost and place tower
            gameManager.DeductCurrency(towerCost);
            Instantiate(towerPrefab, position, rotation, transform);
        }
        else
        {
            // Show notification that player doesn't have enough currency
            if (gameManager)
                gameManager.ShowNotification("Not enough currency to place tower!");
        }
    }

    // Can be called from UI button
    public void PlaceTowerAtRaycast()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Check if ray hits an AR plane
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.GetComponent<ARPlane>() != null)
            {
                // Place tower at hit position
                PlaceTower(hit.point, Quaternion.identity);
            }
        }
    }
}