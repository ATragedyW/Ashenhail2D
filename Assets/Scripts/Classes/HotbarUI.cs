using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HotbarUI : MonoBehaviour
{
    [Header("UI References")]
    public Button abilityButton;
    public Image abilityIcon;
    public TMP_Text abilityNameText;
    public TMP_Text keybindText;
    public Image cooldownOverlay;
    public TMP_Text cooldownText;
    public TMP_Text manaCostText;
    
    [Header("Settings")]
    public KeyCode hotkey = KeyCode.Q;
    public Sprite defaultIcon;
    
    [Header("Visuals")]
    public float flashDuration = 0.2f;
    public Color flashColor = Color.yellow;
    public Color readyColor = Color.white;
    public Color cooldownColor = Color.gray;
    
    private PlayerController player;
    private Image buttonImage;
    private Color originalColor;
    
    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        
        if (player == null)
        {
            Debug.LogError("HotbarUI: PlayerController not found!");
            return;
        }
        
        buttonImage = abilityButton.GetComponent<Image>();
        if (buttonImage != null)
            originalColor = buttonImage.color;
        
        // Set key text
        if (keybindText != null)
            keybindText.text = hotkey.ToString();
        
        // Setup button click
        abilityButton.onClick.RemoveAllListeners();
        abilityButton.onClick.AddListener(UseAbility);
        
        // Initial update
        UpdateDisplay();
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Hotkey input
        if (Input.GetKeyDown(hotkey))
            UseAbility();
        
        // CONTINUOUSLY update cooldown display every frame
        UpdateCooldown();
    }
    
    void UpdateDisplay()
    {
        if (player.currentClass != null)
        {
            // Update from player class
            if (abilityNameText != null)
                abilityNameText.text = player.currentClass.primaryAbilityName;
            
            if (abilityIcon != null)
            {
                abilityIcon.sprite = player.currentClass.classIcon != null 
                    ? player.currentClass.classIcon 
                    : defaultIcon;
                
                // Color based on cooldown
                abilityIcon.color = IsAbilityReady() ? readyColor : cooldownColor;
            }
            
            // Update mana cost
            if (manaCostText != null)
            {
                int cost = player.currentClass.primaryManaCost;
                manaCostText.text = cost.ToString();
                manaCostText.gameObject.SetActive(cost > 0);
            }
        }
        else
        {
            // Default display
            if (abilityNameText != null)
                abilityNameText.text = "No Class";
            
            if (abilityIcon != null && defaultIcon != null)
                abilityIcon.sprite = defaultIcon;
        }
    }
    
    void UpdateCooldown()
    {
        if (player == null || cooldownOverlay == null) return;
        
        // Get current cooldown state from player
        float cooldownPercent = player.GetCooldownPercentage(1);
        float remainingTime = player.GetRemainingCooldownTime(1);
        
        // Debug to see what's happening
        if (remainingTime > 0)
            Debug.Log($"Cooldown: {remainingTime:F1}s ({cooldownPercent:P0})");
        
        // Update cooldown overlay (filled circle)
        cooldownOverlay.fillAmount = cooldownPercent;
        cooldownOverlay.gameObject.SetActive(cooldownPercent > 0.01f);
        
        // Update cooldown text (timer number)
        if (cooldownText != null)
        {
            if (cooldownPercent > 0.01f)
            {
                cooldownText.text = remainingTime.ToString("F1");
                cooldownText.gameObject.SetActive(true);
            }
            else
            {
                cooldownText.text = "";
                cooldownText.gameObject.SetActive(false);
            }
        }
        
        // Update button interactability and icon color
        bool isReady = IsAbilityReady();
        abilityButton.interactable = isReady;
        
        if (abilityIcon != null)
            abilityIcon.color = isReady ? readyColor : cooldownColor;
    }
    
    void UseAbility()
    {
        if (player == null || !IsAbilityReady()) return;
        
        // Use ability through player controller
        player.UsePrimaryAbilityButton();
        
        // Visual feedback
        StartCoroutine(FlashButton());
        
        // Immediately update display to show cooldown started
        UpdateCooldown();
    }
    
    bool IsAbilityReady()
    {
        return player.IsAbilityReady(1);
    }
    
    IEnumerator FlashButton()
    {
        if (buttonImage == null) yield break;
        
        buttonImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        buttonImage.color = originalColor;
    }
    
    public void HighlightButton(int abilitySlot = 1)
    {
        StartCoroutine(FlashButton());
    }
    
    public void RefreshDisplay()
    {
        UpdateDisplay();
        UpdateCooldown();
    }
}