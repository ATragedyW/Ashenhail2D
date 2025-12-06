using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 10f;
    public float lifetime = 4f;
    
    private Vector2 direction;
    private float speed;

    void Start()
    {
        Debug.Log("EnemyProjectile SPAWNED at: " + transform.position);
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 newDirection, float newSpeed)
    {
        direction = newDirection.normalized;
        speed = newSpeed;
        
        Debug.Log($"Projectile SET: Direction={direction}, Speed={speed}");
    }

    void Update()
    {
        // Move the projectile
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        
        // Debug movement every 30 frames to avoid spam
        if (Time.frameCount % 30 == 0)
        {
            Debug.Log($"Projectile MOVING: Position={transform.position}, Direction={direction}");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (collision.CompareTag("Player"))
        {
            // Damage player
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Enemy")) // Destroy on walls, etc.
        {
            Destroy(gameObject);
        }
    }
}
