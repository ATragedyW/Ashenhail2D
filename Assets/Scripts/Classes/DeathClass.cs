using UnityEngine;

[CreateAssetMenu(menuName = "PlayerClass/Necromancy")]
public class NecromancyClass : PlayerClass
{
    [Header("Shadow Bolt Settings")]
    public GameObject shadowBoltPrefab;
    public float shadowBoltSpeed = 12f;
    public float shadowBoltDamage = 35f;
    public float shadowBoltLifetime = 4f;
    public float shadowBoltCooldown = 1.2f;
    public int shadowBoltManaCost = 8;
    
    [Header("Visual Effects")]
    public Color shadowBoltColor = new Color(0.2f, 0f, 0.3f, 1f);
    public GameObject hitEffectPrefab;
    
    [Header("Curse Effect (Optional)")]
    public bool applyCurse = true;
    public float curseDuration = 3f;
    public float curseDamagePerSecond = 5f;

    public override void PrimaryAbility(GameObject player)
    {
        if (player == null)
        {
            Debug.LogError("NecromancyClass: Player GameObject is null!");
            return;
        }

        if (shadowBoltPrefab == null)
        {
            Debug.LogError("NecromancyClass: ShadowBoltPrefab is not assigned!");
            return;
        }

        // Check mana cost
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null && shadowBoltManaCost > 0)
        {
            if (playerController.currentMana < shadowBoltManaCost)
            {
                Debug.Log("Not enough mana for Shadow Bolt!");
                return;
            }
            playerController.currentMana -= shadowBoltManaCost;
        }

        // Get spawn position (offset from player)
        Vector2 spawnPosition = (Vector2)player.transform.position + GetSpawnOffset(player);
        
        // Calculate direction towards mouse
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector2 direction = (mouseWorld - (Vector3)spawnPosition).normalized;
        
        Debug.Log($"Casting Shadow Bolt at {spawnPosition} towards {direction}");
        
        // Spawn shadow bolt
        GameObject bolt = Instantiate(shadowBoltPrefab, spawnPosition, Quaternion.identity);
        
        // Configure shadow bolt
        ShadowBolt shadowBolt = bolt.GetComponent<ShadowBolt>();
        if (shadowBolt == null)
        {
            shadowBolt = bolt.AddComponent<ShadowBolt>();
        }
        
        shadowBolt.speed = shadowBoltSpeed;
        shadowBolt.damage = shadowBoltDamage;
        shadowBolt.lifetime = shadowBoltLifetime;
        shadowBolt.boltColor = shadowBoltColor;
        shadowBolt.hitEffectPrefab = hitEffectPrefab;
        shadowBolt.applyCurse = applyCurse;
        shadowBolt.curseDuration = curseDuration;
        shadowBolt.curseDamagePerSecond = curseDamagePerSecond;
        shadowBolt.caster = player;
        
        shadowBolt.SetDirection(direction);
        
        Debug.Log("Shadow Bolt cast successfully!");
    }

   
    private Vector2 GetSpawnOffset(GameObject player)
    {
        // Calculate offset based on mouse direction
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector2 direction = ((Vector2)mouseWorld - (Vector2)player.transform.position).normalized;
        
        // Offset the spawn position in the direction the player is aiming
        return direction * 0.8f; // 0.8 units away from player
    }
}