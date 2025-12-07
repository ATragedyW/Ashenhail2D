
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text gameOverText;
    public TMP_Text statsText;
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string gameScene = "GameScene";
    
    void Start()
    {
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        
        // Display stats if available
        UpdateStatsDisplay();
    }
    
    void UpdateStatsDisplay()
    {
       
        if (statsText != null)
        {
            string stats = "";
            stats += $"Level: {PlayerPrefs.GetInt("PlayerLevel", 1)}\n";
            stats += $"XP: {PlayerPrefs.GetInt("PlayerXP", 0)}\n";
            stats += $"Enemies Defeated: {PlayerPrefs.GetInt("EnemiesKilled", 0)}";
            
            statsText.text = stats;
        }
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(gameScene);
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}