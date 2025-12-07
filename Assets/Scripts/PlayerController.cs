using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Respawn Settings")]
    public bool isDead = false;
    public int maxLives = 3;
    public int currentLives = 3;
    public float respawnTime = 3f;
    public Transform respawnPoint;
    public bool invulnerableAfterRespawn = true;
    public float invulnerabilityDuration = 2f;
    private bool isInvulnerable = false;
    
    [Header("Death Settings")]
    public float deathMenuDelay = 2f;
    public GameObject deathMenuPanel;
    public TMP_Text deathStatsText;

    [Header("Ability Cooldowns")]
    public bool enableCooldowns = true;
    private float primaryCooldownTimer = 0f;
    private float secondaryCooldownTimer = 0f;
    private float ultimateCooldownTimer = 0f;
    private bool isPrimaryOnCooldown = false;
    private bool isSecondaryOnCooldown = false;
    private bool isUltimateOnCooldown = false;

    private float primaryCooldown = 2f;
    private float secondaryCooldown = 3f;
    private float ultimateCooldown = 10f;

    [Header("UI References")]
    public TMP_Text levelText;
    public TMP_Text xpText;
    public TMP_Text healthText;
    public TMP_Text manaText;
    public TMP_Text livesText;
    
    [Header("UI Events")]
    public UnityEvent OnHealthChangedEvent;
    public UnityEvent OnManaChangedEvent;
    public UnityEvent OnPlayerLevelUpEvent;
    public UnityEvent OnPlayerDiedEvent;
    public UnityEvent OnPlayerRespawnedEvent;
    public System.Action OnStatsChanged;

    [Header("Visual Components")]
    public SpriteRenderer spriteRenderer; 
    public GameObject deathEffectPrefab;
    public AudioClip deathSound;
    public AudioClip respawnSound;

    private HotbarUI hotbarUI;
    private Vector3 initialSpawnPosition;

    void Start()
    {
        initialSpawnPosition = transform.position;
        
        if (respawnPoint == null)
        {
            GameObject respawnGO = new GameObject("RespawnPoint");
            respawnPoint = respawnGO.transform;
            respawnPoint.position = initialSpawnPosition;
        }
       
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false);
        }
        
        if (GameManager.Instance != null && GameManager.Instance.chosenClass != null)
        {
            this.currentClass = GameManager.Instance.chosenClass;
            this.InitializeStatsFromClass();
        }
        
        HotbarUI hotbar = FindAnyObjectByType<HotbarUI>();
        if (hotbar != null)
        {
            hotbar.RefreshDisplay();
        }
       
        InitializeStatsFromClass();
    
        OnStatsChanged?.Invoke();
        UpdateLevelUI();
        UpdateStatsUI();
        UpdateLivesUI();
        
        if (deathMenuPanel != null)
            deathMenuPanel.SetActive(false);
    }

    public void InitializeStatsFromClass()
    {
        if (currentClass != null)
        {
            maxHealth = currentClass.GetMaxHealth(playerLevel);
            currentHealth = maxHealth;
            
            maxMana = currentClass.GetMaxMana(playerLevel);
            currentMana = maxMana;
            
            damage = currentClass.GetAttackPower(playerLevel);
            
            primaryCooldown = currentClass.primaryCooldown;
            secondaryCooldown = currentClass.secondaryCooldown;
            ultimateCooldown = currentClass.ultimateCooldown;
            
            if (spriteRenderer != null)
                spriteRenderer.color = currentClass.classColor;
        }
        else
        {
            maxHealth = 100;
            currentHealth = maxHealth;
            maxMana = 50;
            currentMana = maxMana;
            damage = 10;
        }
    }

    void Update()
    {
        if (isDead) return;
        
        HandleAbilityInput();
        UpdateCooldowns();
        UpdateStatsUI();
    
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(10);
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            GainXP(50);
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(currentHealth);
        }
        
        if (Input.GetKeyDown(KeyCode.F10))
        {
            TakeDamage(currentHealth);
        }
    }

    void UpdateCooldowns()
    {
        if (!enableCooldowns) return;

        if (isPrimaryOnCooldown)
        {
            primaryCooldownTimer -= Time.deltaTime;
            
            if (primaryCooldownTimer <= 0f)
            {
                isPrimaryOnCooldown = false;
                primaryCooldownTimer = 0f;
            }
        }

        if (isSecondaryOnCooldown)
        {
            secondaryCooldownTimer -= Time.deltaTime;
            
            if (secondaryCooldownTimer <= 0f)
            {
                isSecondaryOnCooldown = false;
                secondaryCooldownTimer = 0f;
            }
        }

        if (isUltimateOnCooldown)
        {
            ultimateCooldownTimer -= Time.deltaTime;
            
            if (ultimateCooldownTimer <= 0f)
            {
                isUltimateOnCooldown = false;
                ultimateCooldownTimer = 0f;
            }
        }
    }

    void HandleAbilityInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryUsePrimaryAbility();
        }
    }

    public void UsePrimaryAbilityButton()
    {
        TryUsePrimaryAbility();
    }
    
    public void TryUsePrimaryAbility()
    {
        if (currentClass == null) return;
        
        if (enableCooldowns && isPrimaryOnCooldown)
        {
            StartCoroutine(CooldownFlash(1));
            return;
        }
        
        if (currentClass.primaryManaCost > 0 && !SpendMana(currentClass.primaryManaCost))
        {
            return;
        }
        
        currentClass.PrimaryAbility(gameObject);
        hotbarUI?.HighlightButton(1);
        
        if (enableCooldowns)
        {
            StartPrimaryCooldown();
        }
    }

    void StartPrimaryCooldown()
    {
        isPrimaryOnCooldown = true;
        primaryCooldownTimer = primaryCooldown;
    }

    IEnumerator CooldownFlash(int abilitySlot)
    {
        if (hotbarUI != null)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    public float GetCooldownPercentage(int abilitySlot)
    {
        if (!enableCooldowns) return 0f;
        
        switch (abilitySlot)
        {
            case 1: 
                return isPrimaryOnCooldown ? primaryCooldownTimer / primaryCooldown : 0f;
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
    }

    public void SetAbilityCooldowns(float primaryCD, float secondaryCD, float ultimateCD)
    {
        primaryCooldown = primaryCD;
        secondaryCooldown = secondaryCD;
        ultimateCooldown = ultimateCD;
    }

    void FixedUpdate()
    {
        if (isDead) return;
        
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
            
        UpdateLivesUI();
    }
    
    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = $"Lives: {currentLives}/{maxLives}";
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || isInvulnerable) return;
        
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);
        
        OnHealthChangedEvent?.Invoke();
        OnStatsChanged?.Invoke();
        
        currentClass?.OnHealthChanged(currentHealth, maxHealth, this);
        
        if (spriteRenderer != null)
            StartCoroutine(DamageFlash());
            
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        int oldHealth = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        if (oldHealth != currentHealth)
        {
            OnHealthChangedEvent?.Invoke();
            OnStatsChanged?.Invoke();
            currentClass?.OnHealthChanged(currentHealth, maxHealth, this);
        }
    }
    
    public bool SpendMana(int manaCost)
    {
        if (currentMana >= manaCost)
        {
            currentMana -= manaCost;
            OnManaChangedEvent?.Invoke();
            OnStatsChanged?.Invoke();
            currentClass?.OnManaChanged(currentMana, maxMana, this);
            return true;
        }
        return false;
    }
    
    public void RestoreMana(int manaAmount)
    {
        int oldMana = currentMana;
        currentMana += manaAmount;
        currentMana = Mathf.Min(currentMana, maxMana);
        
        if (oldMana != currentMana)
        {
            OnManaChangedEvent?.Invoke();
            OnStatsChanged?.Invoke();
            currentClass?.OnManaChanged(currentMana, maxMana, this);
        }
    }

    public void GainXP(int xpAmount)
    {
        playerXP += xpAmount;
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
            OnPlayerLevelUpEvent?.Invoke();
        }
    }

    int CalculateXPForNextLevel()
    {
        return Mathf.RoundToInt(100 * Mathf.Pow(playerLevel, 1.5f));
    }

    void LevelUpBenefits()
    {
        if (currentClass != null)
        {
            maxHealth = currentClass.GetMaxHealth(playerLevel);
            maxMana = currentClass.GetMaxMana(playerLevel);
            damage = currentClass.GetAttackPower(playerLevel);
            currentHealth = maxHealth;
            currentMana = maxMana;
        }
        else
        {
            maxHealth += 10;
            maxMana += 5;
            damage += 2;
            currentHealth = maxHealth;
            currentMana = maxMana;
        }
        
        OnHealthChangedEvent?.Invoke();
        OnManaChangedEvent?.Invoke();
        OnStatsChanged?.Invoke();
    }

    void UpdateLevelUI()
    {
        if (levelText != null)
            levelText.text = $"Level: {playerLevel}";
            
        if (xpText != null)
            xpText.text = $"XP: {playerXP}/{xpToNextLevel}";
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
        if (isDead) return;
        
        isDead = true;
        OnPlayerDiedEvent?.Invoke();
        
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
        
        rb.simulated = false;
        
        MonoBehaviour[] components = GetComponents<MonoBehaviour>();
        foreach (var component in components)
        {
            if (component != this && component.enabled)
            {
                component.enabled = false;
            }
        }
        
        currentLives--;
        UpdateLivesUI();
        
        if (currentLives > 0)
        {
            StartCoroutine(RespawnAfterDelay());
        }
        else
        {
            StartCoroutine(ShowDeathMenu());
        }
    }
    
    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }
    
    public void Respawn()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        transform.position = respawnPoint.position;
        
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
        
        rb.simulated = true;
        
        MonoBehaviour[] components = GetComponents<MonoBehaviour>();
        foreach (var component in components)
        {
            if (component != this)
            {
                component.enabled = true;
            }
        }
        
        ResetAllCooldowns();
        
        if (invulnerableAfterRespawn)
        {
            StartCoroutine(InvulnerabilityPeriod());
        }
        
        OnPlayerRespawnedEvent?.Invoke();
        OnStatsChanged?.Invoke();
        UpdateStatsUI();
        isDead = false;
    }
    
    IEnumerator InvulnerabilityPeriod()
    {
        isInvulnerable = true;
        
        if (spriteRenderer == null)
        {
            yield return new WaitForSeconds(invulnerabilityDuration);
            isInvulnerable = false;
            yield break;
        }
        
        float timer = 0f;
        while (timer < invulnerabilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
        
        spriteRenderer.enabled = true;
        isInvulnerable = false;
    }
    
    IEnumerator ShowDeathMenu()
    {
        yield return new WaitForSeconds(deathMenuDelay);
        
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(true);
            
            if (deathStatsText != null)
            {
                string stats = $"Level: {playerLevel}\n";
                stats += $"XP: {playerXP}\n";
                stats += $"Max Health: {maxHealth}\n";
                stats += $"Max Mana: {maxMana}\n";
                stats += $"Enemies Killed: {PlayerPrefs.GetInt("EnemiesKilled", 0)}";
                
                deathStatsText.text = stats;
            }
            
            Time.timeScale = 0.001f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            CreateDeathMenu();
        }
    }
    
    void CreateDeathMenu()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("DeathMenuCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        GameObject panel = new GameObject("DeathMenuPanel");
        panel.transform.SetParent(canvas.transform);
        
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.85f);
        
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        GameObject gameOverText = new GameObject("GameOverText");
        gameOverText.transform.SetParent(panel.transform);
        TMPro.TextMeshProUGUI text = gameOverText.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = "GAME OVER\nKing Defeated!";
        text.fontSize = 48;
        text.color = Color.red;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        
        RectTransform textRect = gameOverText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.7f);
        textRect.anchorMax = new Vector2(0.5f, 0.7f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(600, 100);
        textRect.anchoredPosition = Vector2.zero;
        
        GameObject statsText = new GameObject("StatsText");
        statsText.transform.SetParent(panel.transform);
        TMPro.TextMeshProUGUI stats = statsText.AddComponent<TMPro.TextMeshProUGUI>();
        
        string statsString = $"Level: {playerLevel}\n";
        statsString += $"XP: {playerXP}\n";
        statsString += $"Health: {maxHealth}\n";
        statsString += $"Mana: {maxMana}\n";
        statsString += $"Enemies Killed: {PlayerPrefs.GetInt("EnemiesKilled", 0)}";
        
        stats.text = statsString;
        stats.fontSize = 24;
        stats.color = Color.white;
        stats.alignment = TMPro.TextAlignmentOptions.Center;
        
        RectTransform statsRect = statsText.GetComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0.5f, 0.5f);
        statsRect.anchorMax = new Vector2(0.5f, 0.5f);
        statsRect.pivot = new Vector2(0.5f, 0.5f);
        statsRect.sizeDelta = new Vector2(400, 150);
        statsRect.anchoredPosition = Vector2.zero;
        
        CreateSimpleButton("RestartButton", "Restart Game", 0.3f, () => {
            OnRestartButtonClicked();
        }, panel.transform);
        
        CreateSimpleButton("ClassSelectButton", "Choose New Class", 0.2f, () => {
            OnClassSelectButtonClicked();
        }, panel.transform);
        
        CreateSimpleButton("TitleScreenButton", "Title Screen", 0.1f, () => {
            OnTitleScreenButtonClicked();
        }, panel.transform);
        
        CreateSimpleButton("QuitButton", "Quit to Desktop", 0.0f, () => {
            OnQuitButtonClicked();
        }, panel.transform);
        
        deathMenuPanel = panel;
        Time.timeScale = 0.001f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void CreateSimpleButton(string buttonName, string buttonText, float yPosition, 
        UnityEngine.Events.UnityAction onClick, Transform parent)
    {
        GameObject buttonGO = new GameObject(buttonName);
        buttonGO.transform.SetParent(parent);
        
        Button button = buttonGO.AddComponent<Button>();
        
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.4f, 1f);
        
        GameObject textGO = new GameObject("ButtonText");
        textGO.transform.SetParent(buttonGO.transform);
        TMPro.TextMeshProUGUI text = textGO.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = buttonText;
        text.fontSize = 22;
        text.color = Color.white;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 5);
        textRect.offsetMax = new Vector2(-5, -5);
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, yPosition);
        buttonRect.anchorMax = new Vector2(0.5f, yPosition);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(300, 60);
        buttonRect.anchoredPosition = Vector2.zero;
        
        button.onClick.AddListener(() => {
            onClick?.Invoke();
        });
    }
    
    public void OnRestartButtonClicked()
    {
        Time.timeScale = 1f;
        
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false);
            if (deathMenuPanel.name == "DeathMenuPanel") 
                Destroy(deathMenuPanel);
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void OnClassSelectButtonClicked()
    {
        ReturnToClassSelection();
    }
    
    public void OnTitleScreenButtonClicked()
    {
        Time.timeScale = 1f;
        
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false);
            if (deathMenuPanel.name == "DeathMenuPanel")
                Destroy(deathMenuPanel);
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.chosenClass = null;
        }
        
        SceneManager.LoadScene("TitleScreen");
    }
    
    public void OnQuitButtonClicked()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
    
    public void AddLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            UpdateLivesUI();
        }
    }
    
    public void SetMaxLives(int newMaxLives)
    {
        maxLives = newMaxLives;
        if (currentLives > maxLives)
            currentLives = maxLives;
        UpdateLivesUI();
    }
    
    public void FullHeal()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        OnHealthChangedEvent?.Invoke();
        OnManaChangedEvent?.Invoke();
        OnStatsChanged?.Invoke();
        UpdateStatsUI();
    }
    
    public void ReturnToClassSelection()
    {
        Time.timeScale = 1f;
        
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false);
            if (deathMenuPanel.name == "DeathMenuPanel")
                Destroy(deathMenuPanel);
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToClassSelection();
        }
        else
        {
            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm != null)
            {
                gm.GoToClassSelection();
            }
            else
            {
                if (SceneManager.GetSceneByName("ClassSelection").IsValid())
                {
                    SceneManager.LoadScene("ClassSelection");
                }
                else
                {
                    for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                    {
                        string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                        if (sceneName == "ClassSelection")
                        {
                            SceneManager.LoadScene(i);
                            return;
                        }
                    }
                    SceneManager.LoadScene(0);
                }
            }
        }
    }
}