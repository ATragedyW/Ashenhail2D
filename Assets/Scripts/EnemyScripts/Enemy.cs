using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("XP Reward")]
    public int xpReward = 100;
    
    [Header("Visual Feedback")]
    public SpriteRenderer spriteRenderer;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;
    
    private Color originalColor;
    private bool isFlashing = false;

    void Start()
    {
        currentHealth = maxHealth;
        
        // Auto-get spriteRenderer if not set
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }
        
        // Store original color if we have a sprite renderer
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: No SpriteRenderer found for damage flash!");
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // Flash red when taking damage
        if (spriteRenderer != null && !isFlashing)
        {
            StartCoroutine(DamageFlash());
        }
        
        // Optional: Notify AI script to agro
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
        
        // Flash to red
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        
        // Return to original color (if still alive)
        if (currentHealth > 0)
        {
            spriteRenderer.color = originalColor;
        }
        
        isFlashing = false;
    }
    
    void Die()
    {
        // Grant XP to player
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.GainXP(xpReward);
            Debug.Log($"{gameObject.name} died! Granting {xpReward} XP to player");
        }
        
        // Add any death effects here (particles, sounds, etc.)
        
        // Destroy the enemy
        Destroy(gameObject);
    }
}