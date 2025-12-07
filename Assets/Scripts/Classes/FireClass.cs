using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Fire Mage Class", menuName = "Game/Player Classes/Fire Mage")]
public class FireMageClass : PlayerClass
{
    [Header("Fire Class Abilities")]
    public GameObject fireBoltPrefab;
    
    
    [Header("Ability Properties")]
    public float fireBoltSpeed = 12f;
    public int burnDamage = 5;
    public float burnDuration = 3f;
    public float fireWallDuration = 5f;
    public float meteorDamage = 50f;
    
   
    public override void PrimaryAbility(GameObject player)
    {
        if (player == null || fireBoltPrefab == null) return;
        
        Vector2 playerPos = player.transform.position;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector2 direction = (mouseWorld - (Vector3)playerPos).normalized;
        
        GameObject bolt = Instantiate(fireBoltPrefab, playerPos, Quaternion.identity);
        FireBolt fireBolt = bolt.GetComponent<FireBolt>();
        
        if (fireBolt != null)
        {
            fireBolt.SetDirection(direction);
            fireBolt.speed = fireBoltSpeed;
            
            
        }
        
       
    }
    
    
    
    public void OnEnemyKilledByFire(GameObject enemy)
    {
       
    }
}