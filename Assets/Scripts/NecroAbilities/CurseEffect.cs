using UnityEngine;
using System.Collections;

public class CurseEffect : MonoBehaviour
{
    [Header("Curse Settings")]
    public float duration = 3f;
    public float damagePerSecond = 5f;
    public float slowPercentage = 0.3f; // 30% slow
    
    private EnemyHealth enemyHealth;
    private SimpleEnemyAI enemyAI;
    private float originalSpeed;
    private bool isActive = false;

    public void ApplyCurse(EnemyHealth target, GameObject caster)
    {
        enemyHealth = target;
        enemyAI = target.GetComponent<SimpleEnemyAI>();
        
        if (enemyAI != null)
        {
            originalSpeed = enemyAI.moveSpeed;
            enemyAI.moveSpeed *= (1f - slowPercentage);
        }
        
        isActive = true;
        StartCoroutine(CurseDuration());
        StartCoroutine(DamageOverTime());
    }
    
    IEnumerator CurseDuration()
    {
        yield return new WaitForSeconds(duration);
        RemoveCurse();
    }
    
    IEnumerator DamageOverTime()
    {
        while (isActive && enemyHealth != null)
        {
            yield return new WaitForSeconds(1f);
            
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damagePerSecond);
                
                // Visual effect
                if (enemyHealth.spriteRenderer != null)
                {
                    StartCoroutine(FlashEffect(enemyHealth.spriteRenderer));
                }
            }
        }
    }
    
    IEnumerator FlashEffect(SpriteRenderer sr)
    {
        if (sr == null) yield break;
        
        Color original = sr.color;
        sr.color = Color.Lerp(original, Color.magenta, 0.5f);
        yield return new WaitForSeconds(0.1f);
        sr.color = original;
    }
    
    void RemoveCurse()
    {
        isActive = false;
        
        if (enemyAI != null)
        {
            enemyAI.moveSpeed = originalSpeed;
        }
        
        Destroy(this);
    }
    
    void OnDestroy()
    {
        if (isActive)
        {
            RemoveCurse();
        }
    }
}