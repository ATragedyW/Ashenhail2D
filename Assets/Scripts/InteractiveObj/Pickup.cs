
using UnityEngine;

public class ManaPotionPickup : MonoBehaviour
{
    public int amount = 1;
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem("Mana Potion", amount);
                Destroy(gameObject);
            }
        }
    }
}