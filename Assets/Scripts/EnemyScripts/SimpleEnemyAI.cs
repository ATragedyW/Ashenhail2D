using UnityEngine;
using System.Collections;

public class SimpleEnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float chaseRange = 10f;
    public float moveSpeed = 3f;
    public float stoppingDistance = 1f;
    public float leashRange = 15f;
    
    [Header("State Settings")]
    public float agroBuffer = 2f; 
    public float stateChangeDelay = 0.3f; 
    
    [Header("Physics Settings")]
    public float damping = 2f; 
    
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Vector2 spawnPosition;
    private bool isAggroed = false;
    private float lastStateChangeTime = 0f;
    private EnemyState currentState = EnemyState.Idle;
    
    
    private enum EnemyState
    {
        Idle,
        Chasing,
        Returning,
        Stunned
    }

    void Start()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }
        
        
        rb.linearDamping = damping;
        
        FindPlayer();
    }

    void Update()
    {
        if (playerTransform == null) 
        {
            FindPlayer();
            if (playerTransform == null) return;
        }
        
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        float distanceToSpawn = Vector2.Distance(transform.position, spawnPosition);
        
        
        if (Time.time >= lastStateChangeTime + stateChangeDelay)
        {
            DetermineState(distanceToPlayer, distanceToSpawn);
            lastStateChangeTime = Time.time;
        }
        
        
        ExecuteState(distanceToPlayer);
    }
    
    void FixedUpdate()
    {
       
        ExecuteFixedUpdateMovement();
    }
    
    void DetermineState(float distanceToPlayer, float distanceToSpawn)
    {
        EnemyState newState = currentState;
        
       
        switch (currentState)
        {
            case EnemyState.Idle:
                if (ShouldStartChasing(distanceToPlayer, distanceToSpawn))
                {
                    newState = EnemyState.Chasing;
                    isAggroed = true;
                }
                break;
                
            case EnemyState.Chasing:
                if (ShouldStopChasing(distanceToPlayer, distanceToSpawn))
                {
                    if (distanceToSpawn > leashRange)
                    {
                        newState = EnemyState.Returning;
                        isAggroed = false;
                    }
                    else
                    {
                        newState = EnemyState.Idle;
                    }
                }
                break;
                
            case EnemyState.Returning:
                if (distanceToSpawn < 1f)
                {
                    newState = EnemyState.Idle;
                }
                else if (ShouldStartChasing(distanceToPlayer, distanceToSpawn))
                {
                    newState = EnemyState.Chasing;
                    isAggroed = true;
                }
                break;
        }
        
        
        if (newState != currentState)
        {
            Debug.Log($"{gameObject.name} changing from {currentState} to {newState}");
            currentState = newState;
            
            
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
    
    bool ShouldStartChasing(float distanceToPlayer, float distanceToSpawn)
    {
        
        return (isAggroed || distanceToPlayer <= chaseRange) && 
               distanceToSpawn <= leashRange && 
               distanceToPlayer > stoppingDistance;
    }
    
    bool ShouldStopChasing(float distanceToPlayer, float distanceToSpawn)
    {
       
        return distanceToSpawn > leashRange || 
               (distanceToPlayer > chaseRange + agroBuffer && !isAggroed);
    }
    
    void ExecuteState(float distanceToPlayer)
    {
        switch (currentState)
        {
            case EnemyState.Chasing:
                if (distanceToPlayer > stoppingDistance)
                {
                    ChasePlayer();
                }
                else
                {
                   
                    if (rb.linearVelocity.magnitude > 0.1f)
                    {
                        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.1f);
                    }
                }
                break;
                
            case EnemyState.Returning:
                ReturnToSpawn();
                break;
                
            case EnemyState.Idle:
            case EnemyState.Stunned:
               
                break;
        }
    }
    
    void ExecuteFixedUpdateMovement()
    {
       
        if (currentState == EnemyState.Chasing && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer > stoppingDistance)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                Vector2 targetVelocity = direction * moveSpeed;
                
                
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.2f);
            }
        }
    }
    
    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    
    void ChasePlayer()
    {
        if (playerTransform == null) return;
        
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        
    }
    
    void ReturnToSpawn()
    {
        Vector2 direction = (spawnPosition - (Vector2)transform.position).normalized;
        float distanceToSpawn = Vector2.Distance(transform.position, spawnPosition);
        
        if (distanceToSpawn > 0.5f)
        {
            Vector2 targetVelocity = direction * moveSpeed * 0.7f;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.1f);
            
           
            if (direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
            }
        }
        else
        {
           
            rb.linearVelocity = Vector2.zero;
            transform.position = Vector2.Lerp(transform.position, spawnPosition, 0.1f);
        }
    }
    
   
    public bool IsChasing
    {
        get { return currentState == EnemyState.Chasing; }
    }
    
    
    public void StartChasing()
    {
        isAggroed = true;
        currentState = EnemyState.Chasing;
        Debug.Log(gameObject.name + " agro'd by damage!");
    }
    
    
    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }
    
    System.Collections.IEnumerator StunRoutine(float duration)
    {
        EnemyState previousState = currentState;
        currentState = EnemyState.Stunned;
        rb.linearVelocity = Vector2.zero;
        
        yield return new WaitForSeconds(duration);
        
        
        currentState = previousState;
    }
    
    
    public void ResetAI()
    {
        Debug.Log($"{gameObject.name}: Resetting AI");
        
        
        currentState = EnemyState.Idle;
        isAggroed = false;
        lastStateChangeTime = Time.time;
        
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        
        
        Debug.Log($"{gameObject.name} AI reset to Idle state");
    }
    
   
    public void SetSpawnPosition(Vector2 newSpawnPosition)
    {
        spawnPosition = newSpawnPosition;
    }
    
   
    void OnDrawGizmosSelected()
    {
        
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawSphere(transform.position, chaseRange);
        
       
        if (Application.isPlaying)
        {
            Gizmos.color = new Color(1, 0, 0, 0.1f);
            Gizmos.DrawSphere(spawnPosition, leashRange);
        }
        else
        {
            
            Gizmos.color = new Color(1, 0, 0, 0.05f);
            Gizmos.DrawSphere(transform.position, leashRange);
        }
        
        
        Gizmos.color = new Color(1, 1, 0, 0.05f);
        Gizmos.DrawSphere(transform.position, chaseRange + agroBuffer);
        
       
        if (Application.isPlaying)
        {
            Gizmos.color = currentState switch
            {
                EnemyState.Chasing => Color.yellow,
                EnemyState.Returning => Color.blue,
                EnemyState.Idle => Color.green,
                EnemyState.Stunned => Color.red,
                _ => Color.gray
            };
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}