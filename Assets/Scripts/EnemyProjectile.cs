using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 10f;
    public float lifetime = 4f;
    
    [Header("Visual Effects")]
    public GameObject hitEffect;
    
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
        
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Projectile hit: {collision.gameObject.name} (Tag: {collision.tag})");

        if (collision.CompareTag("Player"))
        {
        
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player == null)
            {
               
                player = collision.GetComponentInParent<PlayerController>();
            }
            
            if (player != null)
            {
                Debug.Log($"Dealing {damage} damage to player!");
                player.TakeDamage((int)damage); 
                
               
                if (hitEffect != null)
                    Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Player hit but PlayerController not found!");
            }
            
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Enemy") && !collision.CompareTag("Projectile")) 
        {
           
            Debug.Log($"Hit non-enemy object: {collision.gameObject.name}");
            
            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);
                
            Destroy(gameObject);
        }
    }
}