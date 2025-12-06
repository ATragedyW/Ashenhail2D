using UnityEngine;

[CreateAssetMenu(menuName = "PlayerClass/Ice")]
public class IceClass : PlayerClass
{
    public GameObject frostBoltPrefab;
    
    public override void PrimaryAbility(GameObject player)
    {
        if (frostBoltPrefab == null)
        {
            Debug.LogError("IceClass: FrostBoltPrefab is not assigned in the Inspector!");
            return;
        }

        if (player == null)
        {
            Debug.LogError("IceClass: Player GameObject is null!");
            return;
        }

        // Get player position
        Vector2 playerPosition = player.transform.position;
        
        // Calculate direction towards mouse
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector2 direction = (mouseWorld - (Vector3)playerPosition).normalized;
        
        Debug.Log($"Spawning FrostBolt at {playerPosition} towards {direction}");
        
        // Spawn frostbolt
        GameObject bolt = Instantiate(frostBoltPrefab, playerPosition, Quaternion.identity);
        
        // Get FrostBolt component
        FrostBolt frostBolt = bolt.GetComponent<FrostBolt>();
        if (frostBolt == null)
        {
            Debug.LogError("IceClass: FrostBolt component is missing on the prefab!");
            return;
        }
        
        // Set direction
        frostBolt.SetDirection(direction);
        Debug.Log("Frost Bolt launched successfully!");
    }

    public override void SecondaryAbility(GameObject player)
    {
        Debug.Log("Frost Nova: Freezes enemies around the player!");
    }

    public override void UltimateAbility(GameObject player)
    {
        Debug.Log("Blizzard: Summons a snowstorm in an area!");
    }
}