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

        
        Vector2 playerPosition = player.transform.position;
        
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector2 direction = (mouseWorld - (Vector3)playerPosition).normalized;
        
        Debug.Log($"Spawning FrostBolt at {playerPosition} towards {direction}");
        
       
        GameObject bolt = Instantiate(frostBoltPrefab, playerPosition, Quaternion.identity);
        
       
        FrostBolt frostBolt = bolt.GetComponent<FrostBolt>();
        if (frostBolt == null)
        {
            Debug.LogError("IceClass: FrostBolt component is missing on the prefab!");
            return;
        }
        
       
        frostBolt.SetDirection(direction);
        Debug.Log("Frost Bolt launched successfully!");
    }

}