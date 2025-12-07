using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName = "New Item";
    public Sprite icon = null;
    [TextArea(3, 5)]
    public string description = "Item description";
    
    [Header("Properties")]
    public int baseValue = 10;
    public bool isStackable = true;
    public int maxStack = 99;
    public bool isUsable = false;
    public bool isSellable = true;
    
    [Header("Shop Settings")]
    public bool canBeSoldInShop = true;
    
    public virtual string GetTooltip()
    {
        return description;
    }
    
    public virtual void Use()
    {
        Debug.Log($"Using {itemName}");
    }
}

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    [Header("Consumable Effects")]
    public int healthRestore = 20;
    public int manaRestore = 10;
    
    public override string GetTooltip()
    {
        string tooltip = base.GetTooltip();
        
        if (healthRestore > 0)
            tooltip += $"\nRestores {healthRestore} HP";
        
        if (manaRestore > 0)
            tooltip += $"\nRestores {manaRestore} MP";
        
        return tooltip;
    }
    
    public override void Use()
    {
        base.Use();
        Debug.Log($"Consumed {itemName}, restored {healthRestore} HP and {manaRestore} MP");
    }
}