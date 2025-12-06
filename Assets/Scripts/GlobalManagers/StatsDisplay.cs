using UnityEngine;
using TMPro;

public class StatsDisplay : MonoBehaviour
{
    [Header("UI References")]
    public GameObject statsPanel;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text attackText;
    public TMP_Text levelText;
    public TMP_Text manaText; // Added mana display
   
    private PlayerController player;
    private bool isDisplaying = false;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        statsPanel.SetActive(false);
        
        if (player == null){
            Debug.LogError("No PlayerController found in scene!");
        }
        player.OnStatsChanged += UpdateStats;


        UpdateStats();
    }
    void OnDestroy()
    {
        if (player != null)

        {
            player.OnStatsChanged -= UpdateStats;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleStatsDisplay();
        }
        
        // Optional: Update stats in real-time while panel is open
        if (isDisplaying)
        {
            UpdateStats();
        }
    }

    void ToggleStatsDisplay()
    {
        isDisplaying = !isDisplaying;
        statsPanel.SetActive(isDisplaying);

        if (isDisplaying)
        {
            UpdateStats();
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void UpdateStats()
    {
        if (player == null) return;

        // Class info
        if (player.currentClass != null)
        {
            nameText.text = $"Class: {player.currentClass.className}";
            nameText.color = player.currentClass.classColor;
        }
        
        // Core stats
        healthText.text = $"Health: {player.currentHealth}/{player.maxHealth}";
        attackText.text = $"Attack: {player.damage}";
        levelText.text = $"Level: {player.playerLevel}";
        
        // Added mana display
        if (manaText != null)
            manaText.text = $"Mana: {player.currentMana}/{player.maxMana}";
       
       Debug.Log($"Stats updated - Health: {player.currentHealth}");
    }

    void PauseGame() => Time.timeScale = 0f;
    void ResumeGame() => Time.timeScale = 1f;
}