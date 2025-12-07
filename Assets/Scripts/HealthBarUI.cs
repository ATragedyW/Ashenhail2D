using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject lowHealthEffect;
    
    [Header("Colors")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;
    
    [Header("Thresholds")]
    [SerializeField] [Range(0, 100)] private int warningThreshold = 50; // 50%
    [SerializeField] [Range(0, 100)] private int criticalThreshold = 25; // 25%
    
    [Header("Effects")]
    [SerializeField] private bool pulseOnLowHealth = true;
    [SerializeField] private float pulseSpeed = 2f;
    
    private PlayerController player;
    private bool isLowHealth = false;
    
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        
        if (player != null)
        {
            // Subscribe to health changes - use PARAMETERLESS method
            player.OnHealthChangedEvent.AddListener(UpdateHealthBarWrapper);
            
            // Initial update
            UpdateHealthBarWrapper(); // Call parameterless version
        }
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = 100;
            healthSlider.minValue = 0;
        }
        
        if (lowHealthEffect != null)
            lowHealthEffect.SetActive(false);
    }
    
    // PARAMETERLESS WRAPPER for UnityEvent
    public void UpdateHealthBarWrapper()
    {
        if (player == null) return;
        UpdateHealthBar(player.currentHealth, player.maxHealth);
    }
    
    void Update()
    {
        // Pulse effect for low health
        if (isLowHealth && pulseOnLowHealth && fillImage != null)
        {
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 0.3f) + 0.7f;
            Color currentColor = fillImage.color;
            currentColor.a = pulse;
            fillImage.color = currentColor;
        }
    }
    
    // Original method with parameters
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0) return; // Prevent division by zero
        
        float healthPercent = (float)currentHealth / maxHealth * 100f;
        
        // Update slider
        if (healthSlider != null)
        {
            healthSlider.value = healthPercent;
        }
        
        // Update text
        if (healthText != null)
        {
            healthText.text = $"HP: {currentHealth}/{maxHealth}";
        }
        
        // Update color based on health percentage
        if (fillImage != null)
        {
            if (healthPercent <= criticalThreshold)
            {
                fillImage.color = criticalColor;
                isLowHealth = true;
                if (lowHealthEffect != null) lowHealthEffect.SetActive(true);
            }
            else if (healthPercent <= warningThreshold)
            {
                fillImage.color = warningColor;
                isLowHealth = false;
                if (lowHealthEffect != null) lowHealthEffect.SetActive(false);
            }
            else
            {
                fillImage.color = healthyColor;
                isLowHealth = false;
                if (lowHealthEffect != null) lowHealthEffect.SetActive(false);
            }
            
            // Reset alpha if not pulsing
            if (!isLowHealth || !pulseOnLowHealth)
            {
                Color color = fillImage.color;
                color.a = 1f;
                fillImage.color = color;
            }
        }
    }
    
    void OnDestroy()
    {
        if (player != null)
        {
            // Remove parameterless wrapper
            player.OnHealthChangedEvent.RemoveListener(UpdateHealthBarWrapper);
        }
    }
}