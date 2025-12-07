using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ControlsManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button controlsButton;
    public Button settingsButton;
    public Button quitButton;
    public Button closeControlsButton;
    
    [Header("Panels")]
    public GameObject controlsPanel;
    
    [Header("Scene Names")]
    public string gameSceneName = "GameScene";
    
    void Start()
    {
        // Setup button events
        startButton.onClick.AddListener(StartGame);
        controlsButton.onClick.AddListener(ShowControls);
        settingsButton.onClick.AddListener(ShowSettings);
        quitButton.onClick.AddListener(QuitGame);
        closeControlsButton.onClick.AddListener(HideControls);
        
        // Hide panels initially
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
        
        // Set cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    void Update()
    {
        // Close controls with Escape
        if (controlsPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideControls();
        }
    }
    
    void StartGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene(gameSceneName);
    }
    
    void ShowControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
            Debug.Log("Controls shown");
        }
    }
    
    void HideControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
            Debug.Log("Controls hidden");
        }
    }
    
    void ShowSettings()
    {
        Debug.Log("Settings button clicked");
       
    }
    
    void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}