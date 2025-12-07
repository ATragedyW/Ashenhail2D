using UnityEngine;

public class FireBolt : MonoBehaviour
{
    [Header("Fire Bolt Settings")]
    public float speed = 12f;
    public float damage = 30f;
    public float lifetime = 3f;
    
    [Header("Visual Settings")]
    public float scale = 0.6f;
    public SpriteRenderer spriteRenderer;
    
    private Vector2 direction;

    void Start()
    {
        transform.localScale = new Vector3(scale, scale, 1f);
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        transform.rotation = Quaternion.identity;
        
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void Update()
    {
        if (direction != Vector2.zero)
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;
        
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Debug.Log($"Fire Bolt hit enemy for {damage} damage!");
        }
        
        Destroy(gameObject);
    }
}