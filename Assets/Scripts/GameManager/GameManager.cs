using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public PlayerClass chosenClass;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager: Created and set to persist");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameManager: Scene loaded - {scene.name}");
        ApplyChosenClassToPlayer();
    }

    void ApplyChosenClassToPlayer()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        
        if (player != null)
        {
            if (chosenClass != null)
            {
                player.currentClass = chosenClass;
                player.InitializeStatsFromClass();
                Debug.Log($"GameManager: Applied {chosenClass.className} to player in {SceneManager.GetActiveScene().name}");
            }
            else
            {
                Debug.LogWarning("GameManager: No class chosen yet - player will use default class");
            }
        }
    }

    public void SetClass(PlayerClass playerClass)
    {
        chosenClass = playerClass;
        Debug.Log($"GameManager: Class set to {playerClass.className}");
    }

    // Method to start the game after class selection
    public void StartGame()
    {
        if (chosenClass != null)
        {
            SceneManager.LoadScene("Test");
        }
        else
        {
            Debug.LogWarning("Please select a class first!");
        }
    }
}