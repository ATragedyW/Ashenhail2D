using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public float shootInterval = 2f;
    public float projectileSpeed = 5f;
    public float attackRange = 10f;
    
    [Header("Visual Feedback")]
    public Transform firePoint;
    
    private Transform player;
    private float shootTimer;
    private EnemyHealth enemyHealth;

    void Start()
    {
        // Find player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Get the EnemyHealth component to check if alive
        enemyHealth = GetComponent<EnemyHealth>();
        
        shootTimer = shootInterval;
        
        if (firePoint == null)
            firePoint = transform;
    }

    void Update()
    {
        // Don't shoot if dead (using EnemyHealth) or no player
        if (enemyHealth != null && enemyHealth.currentHealth <= 0) return;
        if (player == null) return;
        
        // Check if player is in range
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange) return;
        
        // Update shoot timer
        shootTimer -= Time.deltaTime;
        
        if (shootTimer <= 0f)
        {
            ShootAtPlayer();
            shootTimer = shootInterval;
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is null!");
            return;
        }

        Vector2 direction = (player.position - firePoint.position).normalized;
        
        Debug.Log($"Creating projectile at: {firePoint.position}");
        Debug.Log($"Player position: {player.position}");
        Debug.Log($"Direction to player: {direction}");
        
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Debug.Log($"Projectile created: {projectile != null}");
        
        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        if (enemyProjectile != null)
        {
            Debug.Log($"EnemyProjectile component found, setting direction with speed: {projectileSpeed}");
            enemyProjectile.SetDirection(direction, projectileSpeed);
        }
        else
        {
            Debug.LogError("NO EnemyProjectile COMPONENT FOUND ON PREFAB!");
            
            // List all components for debugging
            Component[] allComponents = projectile.GetComponents<Component>();
            foreach (Component comp in allComponents)
            {
                Debug.Log($"Component on projectile: {comp.GetType()}");
            }
        }
        
        Debug.Log($"{gameObject.name} shot at player!");
    }
}