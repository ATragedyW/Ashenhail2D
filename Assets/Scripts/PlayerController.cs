using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Class & Movement")]
    public PlayerClass currentClass;
    public float speed = 50f;
    public Rigidbody2D rb;

    [Header("Player Stats")]
    public string playerName;
    public int playerXP = 0;
    public int playerLevel = 1;
    public int xpToNextLevel = 100;
    public int damage;
    
    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;
    
    [Header("Mana Stats")]
    public int maxMana;
    public int currentMana;

    [Header("Ability Cooldowns")]
    public bool enableCooldowns = true;
    private float primaryCooldownTimer = 0f;
    private float secondaryCooldownTimer = 0f;
    private float ultimateCooldownTimer = 0f;
    private bool isPrimaryOnCooldown = false;
    private bool isSecondaryOnCooldown = false;
    private bool isUltimateOnCooldown = false;

    // Cooldown values (can be overridden by specific class instances)
    private float primaryCooldown = 2f;
    private float secondaryCooldown = 3f;
    private float ultimateCooldown = 10f;

    [Header("UI References")]
    public TMP_Text levelText;
    public TMP_Text xpText;
    public TMP_Text healthText;
    public TMP_Text manaText;
    
    [Header("Visual Components")]
    public SpriteRenderer spriteRenderer; 

    private HotbarUI hotbarUI;
    public System.Action OnStatsChanged;

    void Start()
    {
        hotbarUI = FindAnyObjectByType<HotbarUI>();
        Debug.Log($"Player initializing - Class: {currentClass?.className}");
    
        InitializeStatsFromClass();
    
        // Debug after class initialization
        Debug.Log($"Player stats - Health: {currentHealth}/{maxHealth}, Mana: {currentMana}/{maxMana}");
    
        OnStatsChanged?.Invoke();
        UpdateLevelUI();
        UpdateStatsUI();

        // Initialize cooldown UI if hotbar exists
        if (hotbarUI != null && enableCooldowns)
        {
            //hotbarUI.InitializeCooldownUI(this);
        }
    }

    public void InitializeStatsFromClass()
    {
        if (currentClass != null)
        {
            maxHealth = currentClass.maxHealth;
            currentHealth = maxHealth;
            maxMana = currentClass.maxMana;
            currentMana = maxMana;
            damage = currentClass.attackPower;
            
            // Set class color
            if (spriteRenderer != null)
                spriteRenderer.color = currentClass.classColor;
        }
    }

    void Update()
    {
        HandleAbilityInput();
        UpdateCooldowns();
        
        // Update stats UI every frame
        UpdateStatsUI();
         HandleAbilityInput();
    UpdateCooldowns();
    UpdateStatsUI();
    
    // TEST: Press T to take 10 damage
    if (Input.GetKeyDown(KeyCode.T))
    {
        Debug.Log("=== MANUAL TEST: Taking 100 damage ===");
        TakeDamage(100);
    }
    }

    void UpdateCooldowns()
    {
        if (!enableCooldowns) return;

        // Update primary cooldown
        if (isPrimaryOnCooldown)
        {
            primaryCooldownTimer -= Time.deltaTime;
            UpdateCooldownUI(1, GetCooldownPercentage(1));
            
            if (primaryCooldownTimer <= 0f)
            {
                isPrimaryOnCooldown = false;
                primaryCooldownTimer = 0f;
                UpdateCooldownUI(1, 0f);
            }
        }

        // Update secondary cooldown
        if (isSecondaryOnCooldown)
        {
            secondaryCooldownTimer -= Time.deltaTime;
            UpdateCooldownUI(2, GetCooldownPercentage(2));
            
            if (secondaryCooldownTimer <= 0f)
            {
                isSecondaryOnCooldown = false;
                secondaryCooldownTimer = 0f;
                UpdateCooldownUI(2, 0f);
            }
        }

        // Update ultimate cooldown
        if (isUltimateOnCooldown)
        {
            ultimateCooldownTimer -= Time.deltaTime;
            UpdateCooldownUI(3, GetCooldownPercentage(3));
            
            if (ultimateCooldownTimer <= 0f)
            {
                isUltimateOnCooldown = false;
                ultimateCooldownTimer = 0f;
                UpdateCooldownUI(3, 0f);
            }
        }
    }

    void UpdateCooldownUI(int abilitySlot, float percentage)
    {
        if (hotbarUI != null)
        {
           Debug.Log($"Ability {abilitySlot} CD: {percentage:P0}");
        }
    }

    void HandleAbilityInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryUsePrimaryAbility();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TryUseSecondaryAbility();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TryUseUltimateAbility();
        }
    }

    // Public methods for UI buttons
    public void UsePrimaryAbilityButton()
    {
        TryUsePrimaryAbility();
    }
    
    public void UseSecondaryAbilityButton()
    {
        TryUseSecondaryAbility();
    }
    
    public void UseUltimateAbilityButton()
    {
        TryUseUltimateAbility();
    }

    // Primary Ability
    public void TryUsePrimaryAbility()
    {
        if (currentClass == null) 
        {
            Debug.Log("No class selected!");
            return;
        }
        
        if (!enableCooldowns || !isPrimaryOnCooldown)
        {
            // Cast the ability
            currentClass.PrimaryAbility(gameObject);
            hotbarUI?.HighlightButton(1);
            
            if (enableCooldowns)
            {
                StartPrimaryCooldown();
            }
            
            Debug.Log($"{currentClass.primaryAbilityName} used! Cooldown: {primaryCooldown}s");
        }
        else
        {
            Debug.Log($"{currentClass.primaryAbilityName} on cooldown! {primaryCooldownTimer:F1}s remaining");
            // Visual feedback for cooldown
            StartCoroutine(CooldownFlash(1));
        }
    }

    void StartPrimaryCooldown()
    {
        isPrimaryOnCooldown = true;
        primaryCooldownTimer = primaryCooldown;
        UpdateCooldownUI(1, 1f);
    }

    // Secondary Ability
    public void TryUseSecondaryAbility()
    {
        if (currentClass == null) return;
        
        if (!enableCooldowns || !isSecondaryOnCooldown)
        {
            currentClass.SecondaryAbility(gameObject);
            hotbarUI?.HighlightButton(2);
            
            if (enableCooldowns)
            {
                isSecondaryOnCooldown = true;
                secondaryCooldownTimer = secondaryCooldown;
                UpdateCooldownUI(2, 1f);
            }
            
            Debug.Log($"{currentClass.secondaryAbilityName} used!");
        }
        else
        {
            Debug.Log($"{currentClass.secondaryAbilityName} on cooldown! {secondaryCooldownTimer:F1}s remaining");
            StartCoroutine(CooldownFlash(2));
        }
    }

    // Ultimate Ability
    public void TryUseUltimateAbility()
    {
        if (currentClass == null) return;
        
        if (!enableCooldowns || !isUltimateOnCooldown)
        {
            currentClass.UltimateAbility(gameObject);
            hotbarUI?.HighlightButton(3);
            
            if (enableCooldowns)
            {
                isUltimateOnCooldown = true;
                ultimateCooldownTimer = ultimateCooldown;
                UpdateCooldownUI(3, 1f);
            }
            
            Debug.Log($"{currentClass.ultimateAbilityName} used!");
        }
        else
        {
            Debug.Log($"{currentClass.ultimateAbilityName} on cooldown! {ultimateCooldownTimer:F1}s remaining");
            StartCoroutine(CooldownFlash(3));
        }
    }

    IEnumerator CooldownFlash(int abilitySlot)
    {
        if (hotbarUI != null)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Cooldown utility methods
    public float GetCooldownPercentage(int abilitySlot)
    {
        if (!enableCooldowns) return 0f;
        
        switch (abilitySlot)
        {
            case 1: 
                return isPrimaryOnCooldown ? primaryCooldownTimer / primaryCooldown : 0f;
            case 2: 
                return isSecondaryOnCooldown ? secondaryCooldownTimer / secondaryCooldown : 0f;
            case 3: 
                return isUltimateOnCooldown ? ultimateCooldownTimer / ultimateCooldown : 0f;
            default: return 0f;
        }
    }

    public float GetRemainingCooldownTime(int abilitySlot)
    {
        switch (abilitySlot)
        {
            case 1: return primaryCooldownTimer;
            case 2: return secondaryCooldownTimer;
            case 3: return ultimateCooldownTimer;
            default: return 0f;
        }
    }

    public bool IsAbilityReady(int abilitySlot)
    {
        if (!enableCooldowns) return true;
        
        switch (abilitySlot)
        {
            case 1: return !isPrimaryOnCooldown;
            case 2: return !isSecondaryOnCooldown;
            case 3: return !isUltimateOnCooldown;
            default: return true;
        }
    }

    public void ResetAllCooldowns()
    {
        isPrimaryOnCooldown = false;
        isSecondaryOnCooldown = false;
        isUltimateOnCooldown = false;
        primaryCooldownTimer = 0f;
        secondaryCooldownTimer = 0f;
        ultimateCooldownTimer = 0f;
        
        // Update UI
        for (int i = 1; i <= 3; i++)
        {
            UpdateCooldownUI(i, 0f);
        }
    }

    // Method to set custom cooldowns (for different classes)
    public void SetAbilityCooldowns(float primaryCD, float secondaryCD, float ultimateCD)
    {
        primaryCooldown = primaryCD;
        secondaryCooldown = secondaryCD;
        ultimateCooldown = ultimateCD;
    }

    // Rest of your existing methods...
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2(horizontal, vertical) * speed;
    }

    void UpdateStatsUI()
    {
        if (healthText != null)
            healthText.text = $"HP: {currentHealth}/{maxHealth}";
            
        if (manaText != null)
            manaText.text = $"Mana: {currentMana}/{maxMana}";
    }

    public void GainXP(int xpAmount)
    {
        playerXP += xpAmount;
        Debug.Log($"Gained {xpAmount} XP! Total: {playerXP}/{xpToNextLevel}");
        
        UpdateLevelUI();
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (playerXP >= xpToNextLevel)
        {
            playerLevel++;
            playerXP -= xpToNextLevel;
            xpToNextLevel = CalculateXPForNextLevel();
            
            LevelUpBenefits();
            UpdateLevelUI();
            
            Debug.Log($"LEVEL UP! Now level {playerLevel}. Next level in {xpToNextLevel} XP");
        }
    }

    int CalculateXPForNextLevel()
    {
        return Mathf.RoundToInt(100 * Mathf.Pow(playerLevel, 1.5f));
    }

    void LevelUpBenefits()
    {
        maxHealth += 10;
        maxMana += 5;
        damage += 2;
        
        currentHealth = maxHealth;
        currentMana = maxMana;
        
        Debug.Log($"Level {playerLevel} benefits applied! +10 HP, +5 Mana, +2 Damage");
        
        // Update UI
        UpdateStatsUI();
        OnStatsChanged?.Invoke();
    }

    void UpdateLevelUI()
    {
        if (levelText != null)
            levelText.text = $"Level: {playerLevel}";
            
        if (xpText != null)
            xpText.text = $"XP: {playerXP}/{xpToNextLevel}";
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return;
        
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log($"Player took {damageAmount} damage! Health: {currentHealth}/{maxHealth}");
        OnStatsChanged?.Invoke();
        UpdateStatsUI();
        
        if (spriteRenderer != null)
            StartCoroutine(DamageFlash());
            
        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
            Die();
        }
          Debug.Log($"=== PLAYER TAKING DAMAGE ===");
    Debug.Log($"Method called! Damage: {damageAmount}");
    Debug.Log($"Health Before: {currentHealth}/{maxHealth}");
    
    if (currentHealth <= 0) 
    {
        Debug.Log("Player already dead, ignoring damage");
        return;
    }
    
    currentHealth -= damageAmount;
    currentHealth = Mathf.Max(0, currentHealth);
    
    Debug.Log($"Health After: {currentHealth}/{maxHealth}");
    Debug.Log($"=== END DAMAGE LOG ===");
    
    OnStatsChanged?.Invoke();
    UpdateStatsUI();
    
    if (spriteRenderer != null)
        StartCoroutine(DamageFlash());
        
    if (currentHealth <= 0)
    {
        Debug.Log("Player died!");
        Die();
    }
    }
    
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnStatsChanged?.Invoke();
        UpdateStatsUI();
    }

    IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // Add death effects here
        gameObject.SetActive(false);
    }
}