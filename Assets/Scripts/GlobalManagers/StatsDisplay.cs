using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
    [Header("UI References")]
    public GameObject statsPanel;
    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text attackText;
    public TMP_Text levelText;
    public TMP_Text manaText;
    public TMP_Text xpText;
    
    [Header("Progress Bars")]
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider xpSlider;
    
    [Header("Colors")]
    public Color healthColor = Color.green;
    public Color manaColor = Color.blue;
    public Color xpColor = Color.yellow;
    
    private PlayerController player;
    private bool isDisplaying = false;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        statsPanel.SetActive(false);
        
        if (player == null)
        {
            Debug.LogError("No PlayerController found in scene!");
            return;
        }
        
        // CORRECT: Parameterless UnityEvents subscribe to parameterless methods
        player.OnHealthChangedEvent.AddListener(UpdateHealth);
        player.OnManaChangedEvent.AddListener(UpdateMana);
        
        // This should be += for System.Action (if it's System.Action)
        // If it's UnityEvent, use AddListener instead
        
            player.OnStatsChanged += UpdateStats;
       
        InitializeBars();
        UpdateAllStats();
    }
    
    void InitializeBars()
    {
        if (healthSlider != null)
        {
            Image fill = healthSlider.fillRect?.GetComponent<Image>();
            if (fill != null) fill.color = healthColor;
        }
        if (manaSlider != null)
        {
            Image fill = manaSlider.fillRect?.GetComponent<Image>();
            if (fill != null) fill.color = manaColor;
        }
        if (xpSlider != null)
        {
            Image fill = xpSlider.fillRect?.GetComponent<Image>();
            if (fill != null) fill.color = xpColor;
        }
    }
    
    void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChangedEvent.RemoveListener(UpdateHealth);
            player.OnManaChangedEvent.RemoveListener(UpdateMana);
            
            if (player.OnStatsChanged is System.Action)
                player.OnStatsChanged -= UpdateStats;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleStatsDisplay();
        }
    }

    void ToggleStatsDisplay()
    {
        isDisplaying = !isDisplaying;
        statsPanel.SetActive(isDisplaying);

        if (isDisplaying)
        {
            UpdateAllStats();
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    
    void UpdateAllStats()
    {
        UpdateStats();
        UpdateHealth(); // Call parameterless version
        UpdateMana();   // Call parameterless version
        UpdateXP();
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
        else
        {
            nameText.text = "Class: None";
            nameText.color = Color.white;
        }
        
        // Core stats
        healthText.text = $"Health: {player.currentHealth}/{player.maxHealth}";
        attackText.text = $"Attack: {player.damage}";
        levelText.text = $"Level: {player.playerLevel}";
        manaText.text = $"Mana: {player.currentMana}/{player.maxMana}";
        
        // Update XP text
        if (xpText != null)
            xpText.text = $"XP: {player.playerXP}/{player.xpToNextLevel}";
    }
    
    // PARAMETERLESS VERSION for UnityEvents
    void UpdateHealth()
    {
        if (player == null) return;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = player.maxHealth;
            healthSlider.value = player.currentHealth;
        }
        
        if (healthText != null)
            healthText.text = $"Health: {player.currentHealth}/{player.maxHealth}";
    }
    
    // PARAMETERLESS VERSION for UnityEvents
    void UpdateMana()
    {
        if (player == null) return;
        
        if (manaSlider != null)
        {
            manaSlider.maxValue = player.maxMana;
            manaSlider.value = player.currentMana;
        }
        
        if (manaText != null)
            manaText.text = $"Mana: {player.currentMana}/{player.maxMana}";
    }
    
    void UpdateXP()
    {
        if (player == null) return;
        
        if (xpSlider != null)
        {
            xpSlider.maxValue = player.xpToNextLevel;
            xpSlider.value = player.playerXP;
        }
        
        if (xpText != null)
            xpText.text = $"XP: {player.playerXP}/{player.xpToNextLevel}";
    }

    void PauseGame() => Time.timeScale = 0f;
    void ResumeGame() => Time.timeScale = 1f;
}