using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    
   
    public TMP_Text playerNameText;
    public TMP_Text playerLevelText;
    public TMP_Text playerHealthText;
    public TMP_Text playerManaText;
    
    private PlayerController player;
    
    void Start()
    {
        pauseMenuUI.SetActive(false);
        player = FindFirstObjectByType<PlayerController>(); 
        
        if (player == null)
        {
            Debug.LogWarning("PauseMenu: PlayerController not found!");
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        bool isPaused = !pauseMenuUI.activeSelf;
        pauseMenuUI.SetActive(isPaused);
        
        Time.timeScale = isPaused ? 0f : 1f;
        
        if (isPaused && player != null)
        {
            UpdatePauseMenuUI(); 
        }
    }
    
    void UpdatePauseMenuUI()
    {
        if (player == null) return;
        
        
        if (playerNameText != null)
            playerNameText.text = $"Name: {player.playerName}";
            
        if (playerLevelText != null)
            playerLevelText.text = $"Level: {player.playerLevel}";
            
        if (playerHealthText != null)
            playerHealthText.text = $"Health: {player.currentHealth}/{player.maxHealth}";
            
        if (playerManaText != null)
            playerManaText.text = $"Mana: {player.currentMana}/{player.maxMana}";
    }
    
    
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}