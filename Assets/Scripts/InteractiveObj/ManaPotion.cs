
using UnityEngine;

[CreateAssetMenu(fileName = "NewManaPotion", menuName = "Inventory/ManaPotion")]
public class ManaPotion : Consumable
{
    void OnEnable()
    {
        itemName = "Mana Potion";
        isStackable = true;
        maxStack = 99;
        isUsable = true;
        manaRestore = 30; 
        healthRestore = 0; 
        description = "Restores 30 mana points.";
        baseValue = 50;
    }
    
    public override void Use()
    {
        base.Use();
        
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.RestoreMana(manaRestore);
        }
    }
}