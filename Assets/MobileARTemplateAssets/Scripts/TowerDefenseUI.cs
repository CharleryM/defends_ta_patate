using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDefenseUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject planeDetectionUI;
    public GameObject gameUI;
    public GameObject gameOverUI;
    
    [Header("UI Elements")]
    public Button startGameButton;
    public Button restartButton;
    public TextMeshProUGUI instructionText;
    
    private TowerDefenseGame gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<TowerDefenseGame>();
        
        // Set up button listeners
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameClicked);
        }
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        
        // Show initial UI
        ShowPlaneDetectionUI();
    }
    
    public void ShowPlaneDetectionUI()
    {
        if (planeDetectionUI != null) planeDetectionUI.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(false);
        
        if (instructionText != null)
        {
            instructionText.text = "Scan a flat surface to place your tower defense game";
        }
    }
    
    public void ShowGameUI()
    {
        if (planeDetectionUI != null) planeDetectionUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        if (gameOverUI != null) gameOverUI.SetActive(false);
        
        if (instructionText != null)
        {
            instructionText.text = "Tap to place towers, then tap 'Start Game'";
        }
    }
    
    public void ShowGameOverUI()
    {
        if (planeDetectionUI != null) planeDetectionUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(true);
        
        if (instructionText != null)
        {
            instructionText.text = "Game Over! Tap 'Restart' to play again";
        }
    }
    
    void OnStartGameClicked()
    {
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
        
        if (instructionText != null)
        {
            instructionText.text = "Defend your base from the enemies!";
        }
    }
    
    void OnRestartClicked()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
}