using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthHUD : MonoBehaviour
{
    [Header("Health Display")]
    public Slider healthSlider;
    public TMP_Text healthText;
    public Image healthFill;
    
    [Header("Mana Display")]
    public Slider manaSlider;
    public TMP_Text manaText;
    public Image manaFill;
    
    [Header("Level Display")]
    public TMP_Text levelText;
    public TMP_Text xpText;
    public Slider xpSlider;
    
    [Header("Class Display")]
    public TMP_Text classNameText;
    public Image classIcon;
    
    [Header("Visual Settings")]
    public bool useDynamicColors = true;
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public Color fullManaColor = Color.blue;
    public Color lowManaColor = Color.cyan;
    
    [Header("Low Health Warning")]
    public bool showLowHealthWarning = true;
    public float lowHealthThreshold = 0.3f;
    public Image lowHealthOverlay;
    public float pulseSpeed = 2f;
    
    private PlayerController player;
    private bool isLowHealth = false;
    
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        
        if (player != null)
        {
            // Subscribe to SAME events as StatsDisplay
            player.OnHealthChangedEvent.AddListener(UpdateHealth);
            player.OnManaChangedEvent.AddListener(UpdateMana);
            player.OnStatsChanged += UpdateAllDisplays;
            
            // Initial update - using same method as StatsDisplay
            UpdateAllDisplays();
        }
        
        if (lowHealthOverlay != null)
            lowHealthOverlay.gameObject.SetActive(false);
    }
    
    void Update()
    {
        // Pulse effect for low health
        if (isLowHealth && showLowHealthWarning && lowHealthOverlay != null)
        {
            float alpha = Mathf.PingPong(Time.time * pulseSpeed, 0.4f) + 0.1f;
            Color color = lowHealthOverlay.color;
            color.a = alpha;
            lowHealthOverlay.color = color;
        }
    }
    
    // SAME METHOD NAME as StatsDisplay for consistency
    void UpdateAllDisplays()
    {
        UpdateHealth();
        UpdateMana();
        UpdateLevel();
        UpdateXP();
        UpdateClassInfo(); // Optional: Show class info in HUD
    }
    
    // SAME as StatsDisplay.UpdateHealth()
    void UpdateHealth()
    {
        if (player == null) return;
        
        // Update slider - EXACT same logic as StatsDisplay
        if (healthSlider != null)
        {
            healthSlider.maxValue = player.maxHealth;
            healthSlider.value = player.currentHealth;
        }
        
        // Update text - EXACT same format as StatsDisplay
        if (healthText != null)
        {
            healthText.text = $"HP: {player.currentHealth}/{player.maxHealth}";
            
            // Color coding - same as StatsDisplay
            float healthPercent = (float)player.currentHealth / player.maxHealth;
            healthText.color = healthPercent <= lowHealthThreshold ? Color.red : Color.white;
        }
        
        // Update fill color - dynamic like StatsDisplay
        if (healthFill != null && useDynamicColors)
        {
            float healthPercent = (float)player.currentHealth / player.maxHealth;
            healthFill.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
            
            // Low health warning
            isLowHealth = healthPercent <= lowHealthThreshold;
            
            if (lowHealthOverlay != null)
            {
                lowHealthOverlay.gameObject.SetActive(isLowHealth && showLowHealthWarning);
                if (!isLowHealth)
                {
                    Color color = lowHealthOverlay.color;
                    color.a = 0f;
                    lowHealthOverlay.color = color;
                }
            }
        }
    }
    
    // SAME as StatsDisplay.UpdateMana()
    void UpdateMana()
    {
        if (player == null) return;
        
        // Update slider - EXACT same logic
        if (manaSlider != null)
        {
            manaSlider.maxValue = player.maxMana;
            manaSlider.value = player.currentMana;
        }
        
        // Update text - EXACT same format
        if (manaText != null)
        {
            manaText.text = $"MP: {player.currentMana}/{player.maxMana}";
            
            // Color coding
            float manaPercent = (float)player.currentMana / player.maxMana;
            manaText.color = manaPercent <= 0.2f ? Color.cyan : Color.white;
        }
        
        // Update fill color - dynamic
        if (manaFill != null && useDynamicColors)
        {
            float manaPercent = (float)player.currentMana / player.maxMana;
            manaFill.color = Color.Lerp(lowManaColor, fullManaColor, manaPercent);
        }
    }
    
    // SAME as StatsDisplay.UpdateLevel()
    void UpdateLevel()
    {
        if (player == null || levelText == null) return;
        
        levelText.text = $"Lvl {player.playerLevel}";
        levelText.color = Color.Lerp(Color.white, Color.yellow, 0.7f);
    }
    
    // SAME as StatsDisplay.UpdateXP()
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
    
    // Optional: Add class info to HUD (matches StatsDisplay)
    void UpdateClassInfo()
    {
        if (player == null || player.currentClass == null) return;
        
        if (classNameText != null)
        {
            classNameText.text = player.currentClass.className;
            classNameText.color = player.currentClass.classColor;
        }
        
        if (classIcon != null && player.currentClass.classIcon != null)
        {
            classIcon.sprite = player.currentClass.classIcon;
            classIcon.gameObject.SetActive(true);
        }
    }
    
    void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChangedEvent.RemoveListener(UpdateHealth);
            player.OnManaChangedEvent.RemoveListener(UpdateMana);
            player.OnStatsChanged -= UpdateAllDisplays;
        }
    }
}