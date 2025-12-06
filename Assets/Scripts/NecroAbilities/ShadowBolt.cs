using UnityEngine;
using System.Collections;

public class ShadowBolt : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 12f;
    public float damage = 35f;
    public float lifetime = 4f;
    
    [Header("Visual Settings")]
    public Color boltColor = new Color(0.3f, 0f, 0.5f, 1f);
    public GameObject hitEffectPrefab;
    
    [Header("Curse Effect")]
    public bool applyCurse = true;
    public float curseDuration = 3f;
    public float curseDamagePerSecond = 5f;
    
    [Header("References")]
    public GameObject caster;
    
    private Vector2 direction;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;
    private bool hasHit = false;
    private bool isDestroying = false;

    void Start()
    {
        InitializeComponents();
        SetupVisuals();
        Destroy(gameObject, lifetime);
        
        // If direction hasn't been set externally, calculate based on mouse
        if (direction == Vector2.zero && caster != null)
        {
            CalculateMouseDirection();
        }
    }

    void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = boltColor;
        }
    }

    void SetupVisuals()
    {
        SetupLineRendererTrail();
    }

    void SetupLineRendererTrail()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = boltColor;
        lineRenderer.endColor = new Color(boltColor.r, boltColor.g, boltColor.b, 0f);
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.useWorldSpace = true;
        lineRenderer.numCornerVertices = 5;
        lineRenderer.numCapVertices = 5;
    }

    void Update()
    {
        // Update trail position
        if (lineRenderer != null && !hasHit)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position - (Vector3)(direction * 0.8f));
        }
        
        // Pulsing effect
        if (spriteRenderer != null && !hasHit)
        {
            float pulse = Mathf.Sin(Time.time * 8f) * 0.15f + 0.85f;
            spriteRenderer.color = new Color(
                boltColor.r * pulse,
                boltColor.g * pulse,
                boltColor.b * pulse,
                boltColor.a
            );
        }
    }

    // Method 1: Set direction externally (called from NecromancyClass)
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        ApplyMovement();
    }
    
    // Method 2: Calculate direction based on mouse position
    public void CalculateMouseDirection()
    {
        if (caster == null) return;
        
        // Get mouse position in world coordinates
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        
        // Calculate direction from caster to mouse
        direction = (mouseWorldPos - caster.transform.position).normalized;
        
        ApplyMovement();
    }
    
    // Method 3: Set direction to specific target
    public void SetTarget(Vector3 targetPosition)
    {
        direction = (targetPosition - transform.position).normalized;
        ApplyMovement();
    }
    
    void ApplyMovement()
    {
        // Rotate to face direction
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        // Apply velocity
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            // Fallback if no rigidbody
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // Ensure constant speed in FixedUpdate
        if (!hasHit && rb != null && direction != Vector2.zero)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit || isDestroying) return;
        
        // Don't hit the caster, player, or other shadow bolts
        if (collision.gameObject == caster || 
            collision.CompareTag("Player") || 
            collision.GetComponent<ShadowBolt>() != null)
            return;
        
        hasHit = true;
        isDestroying = true;
        
        // Apply damage to enemies
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Debug.Log($"Shadow Bolt hit enemy for {damage} damage!");
            
            // Apply curse effect
            if (applyCurse)
            {
                ApplyCurse(enemyHealth);
            }
        }
        else
        {
            Debug.Log($"Shadow Bolt hit: {collision.gameObject.name}");
        }
        
        // Create hit effect
        CreateHitEffect();
        
        // Clean up and destroy
        CleanupAndDestroy();
    }

    void ApplyCurse(EnemyHealth enemy)
    {
        StartCoroutine(CurseCoroutine(enemy));
    }
    
    IEnumerator CurseCoroutine(EnemyHealth enemy)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < curseDuration && enemy != null)
        {
            yield return new WaitForSeconds(1f);
            
            if (enemy != null)
            {
                enemy.TakeDamage(curseDamagePerSecond);
                Debug.Log($"Curse dealt {curseDamagePerSecond} damage!");
            }
            
            elapsedTime += 1f;
        }
    }

    void CreateHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        else
        {
            CreateDefaultHitEffect();
        }
    }
    
    void CreateDefaultHitEffect()
    {
        GameObject effect = new GameObject("ShadowHitEffect");
        effect.transform.position = transform.position;
        
        SpriteRenderer sr = effect.AddComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            sr.sprite = spriteRenderer.sprite;
        }
        sr.color = new Color(0.5f, 0f, 0.8f, 0.7f);
        sr.sortingOrder = 10;
        
        StartCoroutine(AnimateHitEffect(effect.transform, sr));
        Destroy(effect, 0.8f);
    }
    
    IEnumerator AnimateHitEffect(Transform effectTransform, SpriteRenderer sr)
    {
        float duration = 0.8f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 endScale = Vector3.one * 2f;
        
        while (elapsed < duration && sr != null)
        {
            float t = elapsed / duration;
            
            // Expand
            effectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            // Fade out
            Color c = sr.color;
            c.a = Mathf.Lerp(0.7f, 0f, t);
            sr.color = c;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void CleanupAndDestroy()
    {
        // Disable visuals
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
        
        if (lineRenderer != null)
            lineRenderer.enabled = false;
        
        // Stop movement
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        
        // Destroy after a small delay
        Destroy(gameObject, 0.1f);
    }

    void OnDestroy()
    {
        isDestroying = true;
        
        // Clean up components
        if (lineRenderer != null)
        {
            Destroy(lineRenderer);
        }
        
        // Stop any coroutines
        StopAllCoroutines();
    }
}