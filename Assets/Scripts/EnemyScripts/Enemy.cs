using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("XP Reward")]
    public int xpReward = 100;

    [Header("Respawn Settings")]
    public bool canRespawn = true;
    public float respawnTime = 10f;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    
    [Header("Visual Feedback")]
    public SpriteRenderer spriteRenderer;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;
    
    [Header("Drop Settings")]
    public Item manaPotionItem; 
    public GameObject manaPotionWorldPrefab; 
    public bool dropDirectlyToInventory = true;
    public float dropChance = 0.3f;
    public int minDrops = 1;
    public int maxDrops = 3;
    
    private Color originalColor;
    private bool isFlashing = false;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        if (spriteRenderer != null && !isFlashing)
        {
            StartCoroutine(DamageFlash());
        }
        
        SimpleEnemyAI ai = GetComponent<SimpleEnemyAI>();
        if (ai != null && !ai.IsChasing)
        {
            ai.StartChasing();
        }
        
        if (currentHealth <= 0) 
        {
            Die();
        }
    }
    
    IEnumerator DamageFlash()
    {
        isFlashing = true;
        
        if (spriteRenderer == null) yield break;
        
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        
        if (currentHealth > 0)
        {
            spriteRenderer.color = originalColor;
        }
        
        isFlashing = false;
    }
    
    void Die()
    {
        if (isDead) return;
        isDead = true;

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.GainXP(xpReward);
            Debug.Log($"{gameObject.name} died! Granting {xpReward} XP to player");
        }
        
        DropManaPotions();
        
        DisableEnemy();
        
        if (canRespawn)
        {
            StartCoroutine(RespawnTimer());
        }
        else
        {
            Destroy(gameObject, 2f); 
        }
        PlayerPrefs.SetInt("EnemiesKilled", PlayerPrefs.GetInt("EnemiesKilled", 0) + 1);
    }
    
    void DropManaPotions()
    {
        if (Random.value > dropChance) return;
        
        int dropCount = Random.Range(minDrops, maxDrops + 1);
        
        if (dropDirectlyToInventory)
        {
            for (int i = 0; i < dropCount; i++)
            {
                AddManaPotionToInventory();
            }
            
            Debug.Log($"{gameObject.name} dropped {dropCount} mana potion(s) to inventory");
        }
        else
        {
            
            if (manaPotionWorldPrefab != null)
            {
                for (int i = 0; i < dropCount; i++)
                {
                    DropManaPotionInWorld();
                }
                Debug.Log($"{gameObject.name} dropped {dropCount} mana potion(s) in world");
            }
            else
            {
                Debug.LogWarning("Cannot drop in world - no manaPotionWorldPrefab assigned!");
            }
        }
    }
    
    void AddManaPotionToInventory()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager.Instance is null!");
            return;
        }
        
       
        if (manaPotionItem != null)
        {
            bool added = InventoryManager.Instance.AddItem(manaPotionItem, 1);
            
            if (added)
            {
                InventoryHUD hud = FindAnyObjectByType<InventoryHUD>();
                if (hud != null)
                {
                    hud.ShowNotification("Found Mana Potion!");
                }
            }
        }
        else
        {
          
           
            InventoryManager.Instance.AddItem("Mana Potion", 1);
        }
    }
    
    void DropManaPotionInWorld()
    {
        if (manaPotionWorldPrefab == null) return;
        
        Vector3 dropPosition = transform.position;
        dropPosition.x += Random.Range(-1f, 1f);
        dropPosition.y += Random.Range(-0.5f, 0.5f);
        
        GameObject manaPotion = Instantiate(manaPotionWorldPrefab, dropPosition, Quaternion.identity);
        
        Rigidbody2D rb = manaPotion.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 force = new Vector2(Random.Range(-1f, 1f), Random.Range(2f, 4f));
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
    
    void DisableEnemy()
    {
        SimpleEnemyAI ai = GetComponent<SimpleEnemyAI>();
        if (ai != null) ai.enabled = false;
        
        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null) shooter.enabled = false;
        
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D != null) collider2D.enabled = false;
        
        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }
    
    void EnableEnemy()
    {
        currentHealth = maxHealth;
        isDead = false;
        
        SimpleEnemyAI ai = GetComponent<SimpleEnemyAI>();
        if (ai != null) 
        {
            ai.enabled = true;
            ai.ResetAI();
        }
        
        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null) shooter.enabled = true;
        
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D != null) collider2D.enabled = true;
        
        if (spriteRenderer != null) 
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }
        
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
    }
    
    IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(respawnTime);
        
        if (IsPlayerTooClose())
        {
            yield return new WaitForSeconds(5f);
        }
        
        EnableEnemy();
    }
    
    bool IsPlayerTooClose()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player == null) return false;
        
        float safeDistance = 5f;
        float distanceToPlayer = Vector3.Distance(spawnPosition, player.transform.position);
        
        return distanceToPlayer < safeDistance;
    }
}