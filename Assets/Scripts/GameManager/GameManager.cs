using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Player Class")]
    public PlayerClass chosenClass;
    
    [Header("Scene Names")]
    public string classSelectionScene = "ClassSelection";
    public string gameScene = "Test"; 
    public string mainMenuScene = "TitleScreen"; 
    
    void Awake()
    {
        Debug.Log($"GameManager Awake in scene: {SceneManager.GetActiveScene().name}");
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager initialized with DontDestroyOnLoad");
        }
        else
        {
            Debug.Log("Destroying duplicate GameManager");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        Debug.Log($"GameManager Start - Scene: {SceneManager.GetActiveScene().name}");
        Debug.Log($"GameManager Instance: {Instance != null}");
    }
    
    public void SetClass(PlayerClass playerClass)
    {
        chosenClass = playerClass;
        Debug.Log($"Selected class: {chosenClass?.className ?? "NULL"}");
    }
    
    public void StartGame()
    {
        if (!string.IsNullOrEmpty(gameScene))
        {
            Debug.Log($"Loading game scene: {gameScene}");
            SceneManager.LoadScene(gameScene);
        }
        else
        {
            Debug.LogError("Game scene name is not set!");
        }
    }
    
    public void GoToClassSelection()
    {
        Debug.Log("=== GameManager.GoToClassSelection() called ===");
        Debug.Log($"Current scene: {SceneManager.GetActiveScene().name}");
        Debug.Log($"Target scene: {classSelectionScene}");
        Debug.Log($"GameManager.Instance exists: {Instance != null}");
        Debug.Log($"GameManager.Instance.chosenClass: {Instance?.chosenClass?.className ?? "NULL"}");
        
       
        chosenClass = null;
          
        Time.timeScale = 1f;
        
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        
        if (!string.IsNullOrEmpty(classSelectionScene))
        {
            Debug.Log($"Loading class selection scene: {classSelectionScene}");
            SceneManager.LoadScene(classSelectionScene);
        }
        else
        {
            Debug.LogError("Class selection scene name is not set!");
            
            
            LoadClassSelectionFallback();
        }
    }
    
    void LoadClassSelectionFallback()
    {
        Debug.Log("Trying fallback method to find class selection scene...");
        
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            Debug.Log($"Checking scene {i}: {sceneName}");
            
            if (sceneName.ToLower().Contains("class") || 
                sceneName.ToLower().Contains("selection") ||
                sceneName.ToLower().Contains("choose"))
            {
                Debug.Log($"Found potential class selection scene: {sceneName} at index {i}");
                SceneManager.LoadScene(i);
                return;
            }
        }
        
        
        Debug.LogWarning("No class selection scene found. Loading first scene.");
        SceneManager.LoadScene(0);
    }
    
    public void ResetGame()
    {
        Debug.Log("GameManager: Resetting game data...");
        
       
        PlayerPrefs.DeleteKey("EnemiesKilled");
        PlayerPrefs.DeleteKey("PlayerLevel");
        PlayerPrefs.DeleteKey("PlayerXP");
        PlayerPrefs.DeleteKey("PlayerMaxHealth");
        PlayerPrefs.DeleteKey("PlayerMaxMana");
        PlayerPrefs.Save();
        
        
        chosenClass = null;
        
        Debug.Log("Game data reset complete");
    }
    
    public void RestartGame()
    {
        Debug.Log("GameManager: Restarting game...");
        
        
        Time.timeScale = 1f;
        
       
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitGame()
    {
        Debug.Log("GameManager: Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}