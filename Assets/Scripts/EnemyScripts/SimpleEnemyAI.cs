using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float chaseRange = 10f;
    public float moveSpeed = 3f;
    public float stoppingDistance = 1f;
    public float leashRange = 15f;
    
    [Header("State Settings")]
    public float agroBuffer = 2f; // Extra range before losing agro
    public float stateChangeDelay = 0.3f; // Delay before changing states
    
    [Header("Physics Settings")]
    public float damping = 2f; // Replaces drag - slows down movement
    
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Vector2 spawnPosition;
    private bool isAggroed = false;
    private float lastStateChangeTime = 0f;
    private EnemyState currentState = EnemyState.Idle;
    
    // Enum for clear state management
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
        
        // FIXED: Use linearDamping instead of deprecated drag
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
        
        // Only check state transitions occasionally (not every frame)
        if (Time.time >= lastStateChangeTime + stateChangeDelay)
        {
            DetermineState(distanceToPlayer, distanceToSpawn);
            lastStateChangeTime = Time.time;
        }
        
        // Execute current state
        ExecuteState(distanceToPlayer);
    }
    
    void FixedUpdate()
    {
        // Smooth physics movement in FixedUpdate
        ExecuteFixedUpdateMovement();
    }
    
    void DetermineState(float distanceToPlayer, float distanceToSpawn)
    {
        EnemyState newState = currentState;
        
        // Check for state transitions
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
        
        // Only change state if different
        if (newState != currentState)
        {
            Debug.Log($"{gameObject.name} changing from {currentState} to {newState}");
            currentState = newState;
            
            // Clear velocity on state change
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
    
    bool ShouldStartChasing(float distanceToPlayer, float distanceToSpawn)
    {
        // Start chasing if:
        // 1. We're agroed OR player is in chase range
        // 2. We're within leash range
        // 3. Player is not too close (stopping distance)
        return (isAggroed || distanceToPlayer <= chaseRange) && 
               distanceToSpawn <= leashRange && 
               distanceToPlayer > stoppingDistance;
    }
    
    bool ShouldStopChasing(float distanceToPlayer, float distanceToSpawn)
    {
        // Stop chasing if:
        // 1. We're beyond leash range OR
        // 2. Player is beyond chase range + buffer AND we're not agroed
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
                    // Stop when close enough
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
                // No movement
                break;
        }
    }
    
    void ExecuteFixedUpdateMovement()
    {
        // Smooth movement in FixedUpdate for chasing
        if (currentState == EnemyState.Chasing && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer > stoppingDistance)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                Vector2 targetVelocity = direction * moveSpeed;
                
                // Smooth velocity changes
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
        // Velocity is set in FixedUpdate for smoothness
    }
    
    void ReturnToSpawn()
    {
        Vector2 direction = (spawnPosition - (Vector2)transform.position).normalized;
        float distanceToSpawn = Vector2.Distance(transform.position, spawnPosition);
        
        if (distanceToSpawn > 0.5f)
        {
            Vector2 targetVelocity = direction * moveSpeed * 0.7f;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.1f);
            
            // Smooth rotation
            if (direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
            }
        }
        else
        {
            // Close enough to spawn
            rb.linearVelocity = Vector2.zero;
            transform.position = Vector2.Lerp(transform.position, spawnPosition, 0.1f);
        }
    }
    
    // Public property for other scripts
    public bool IsChasing
    {
        get { return currentState == EnemyState.Chasing; }
    }
    
    // Method to force agro (called when taking damage)
    public void StartChasing()
    {
        isAggroed = true;
        currentState = EnemyState.Chasing;
        Debug.Log(gameObject.name + " agro'd by damage!");
    }
    
    // Method to stun the enemy (stop movement temporarily)
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
        
        // Return to previous state
        currentState = previousState;
    }
    
    // Visualize ranges in editor
    void OnDrawGizmosSelected()
    {
        // Chase range
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawSphere(transform.position, chaseRange);
        
        // Leash range (only in play mode when spawnPosition is set)
        if (Application.isPlaying)
        {
            Gizmos.color = new Color(1, 0, 0, 0.1f);
            Gizmos.DrawSphere(spawnPosition, leashRange);
        }
        else
        {
            // In editor, draw around current position
            Gizmos.color = new Color(1, 0, 0, 0.05f);
            Gizmos.DrawSphere(transform.position, leashRange);
        }
        
        // Agro buffer
        Gizmos.color = new Color(1, 1, 0, 0.05f);
        Gizmos.DrawSphere(transform.position, chaseRange + agroBuffer);
        
        // State indicator
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