using UnityEngine;

public class FrostBolt : MonoBehaviour
{
    [Header("Frost Bolt Settings")]
    public float speed = 10f;
    public float damage = 25f;
    public float lifetime = 3f;
    
    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // ADD THIS METHOD - Your IceClass is calling it!
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        
        // Rotate to face direction
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void Update()
    {
        // Simple movement
        if (direction != Vector2.zero)
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;
        
        // Check if we hit an enemy
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Debug.Log($"Frost Bolt hit enemy for {damage} damage!");
        }
        else
        {
            Debug.Log($"Frost Bolt hit: {collision.gameObject.name} (not an enemy)");
        }
        
        Destroy(gameObject);
    }
}