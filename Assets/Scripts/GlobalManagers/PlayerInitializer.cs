using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== PlayerInitializer Start() ===");
        
        // 1. Find player
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("No PlayerController found in scene!");
            return;
        }
        
        Debug.Log($"Found player: {player.gameObject.name}");
        
        // 2. Wait one frame to ensure GameManager is ready
        StartCoroutine(InitializePlayer(player));
    }
    
    System.Collections.IEnumerator InitializePlayer(PlayerController player)
    {
        // Wait one frame to ensure GameManager's OnSceneLoaded has run
        yield return null;
        
        Debug.Log($"GameManager.Instance exists: {GameManager.Instance != null}");
        
        // 3. Use GameManager's chosen class if available
        if (GameManager.Instance != null && GameManager.Instance.chosenClass != null)
        {
            player.currentClass = GameManager.Instance.chosenClass;
            player.InitializeStatsFromClass();
            Debug.Log($"Player initialized with class from GameManager: {GameManager.Instance.chosenClass.className}");
        }
        else
        {
            Debug.LogWarning("GameManager or chosenClass is null. Checking player's current class...");
            
            // If player already has a class (from inspector), use it
            if (player.currentClass == null)
            {
                Debug.LogError("Player has no class assigned! Will use default.");
                
                // Load default class from Resources
                PlayerClass defaultClass = Resources.Load<PlayerClass>("Classes/Warrior");
                if (defaultClass != null)
                {
                    player.currentClass = defaultClass;
                    player.InitializeStatsFromClass();
                    Debug.Log($"Assigned default class: {defaultClass.className}");
                }
            }
        }
        
        // 4. Update hotbar
        HotbarUI hotbar = FindAnyObjectByType<HotbarUI>();
        if (hotbar != null)
        {
            hotbar.RefreshDisplay();
            Debug.Log("Hotbar refreshed");
        }
        
        Debug.Log("=== End PlayerInitializer ===");
    }
}