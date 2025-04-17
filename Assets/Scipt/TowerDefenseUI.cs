using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDefenseUI : MonoBehaviour
{
    [Header("Tower Selection")]
    [SerializeField] private GameObject towerSelectionPanel;
    [SerializeField] private TowerPlacement towerPlacement;
    [SerializeField] private GameManager gameManager;
    
    [Header("Tower Buttons")]
    [SerializeField] private Button basicTowerButton;
    [SerializeField] private Button missileTowerButton;
    [SerializeField] private Button laserTowerButton;
    
    [Header("Game HUD")]
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button closeSelectionButton;
    
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject basicTowerPrefab;
    [SerializeField] private GameObject missileTowerPrefab;
    [SerializeField] private GameObject laserTowerPrefab;
    
    private Camera mainCamera;
    private GameObject selectedTowerPrefab;

    private void Start()
    {
        mainCamera = Camera.main;
        
        // Initialize UI state
        if (towerSelectionPanel)
            towerSelectionPanel.SetActive(false);
            
        if (gameOverPanel)
            gameOverPanel.SetActive(false);
            
        // Setup button listeners
        if (basicTowerButton)
            basicTowerButton.onClick.AddListener(() => SelectTower(basicTowerPrefab));
            
        if (missileTowerButton)
            missileTowerButton.onClick.AddListener(() => SelectTower(missileTowerPrefab));
            
        if (laserTowerButton)
            laserTowerButton.onClick.AddListener(() => SelectTower(laserTowerPrefab));
            
        if (closeSelectionButton)
            closeSelectionButton.onClick.AddListener(CloseTowerSelection);
            
        if (restartButton)
            restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowTowerSelection()
    {
        if (towerSelectionPanel)
            towerSelectionPanel.SetActive(true);
    }

    public void CloseTowerSelection()
    {
        if (towerSelectionPanel)
            towerSelectionPanel.SetActive(false);
            
        selectedTowerPrefab = null;
    }

    private void SelectTower(GameObject towerPrefab)
    {
        selectedTowerPrefab = towerPrefab;
        
        // Update tower placement reference to this prefab
        if (towerPlacement)
        {
            // Access the towerPrefab field using reflection (since it's private)
            var field = towerPlacement.GetType().GetField("towerPrefab", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic);
                
            if (field != null)
                field.SetValue(towerPlacement, selectedTowerPrefab);
                
            // Call the placement method
            towerPlacement.PlaceTowerAtRaycast();
        }
        
        // Close the selection panel
        CloseTowerSelection();
    }

    public void RestartGame()
    {
        if (gameManager)
            gameManager.RestartGame();
    }

    public void ShowGameOver()
    {
        if (gameOverPanel)
            gameOverPanel.SetActive(true);
    }
}