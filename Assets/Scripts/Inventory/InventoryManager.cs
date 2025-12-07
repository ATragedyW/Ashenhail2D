using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 20;
    
    [System.Serializable]
    public class InventorySlot
    {
        public Item item;
        public int quantity;
        
        public bool IsEmpty => item == null || quantity <= 0;
        public bool IsFull => item != null && quantity >= item.maxStack;
        
        public void Clear()
        {
            item = null;
            quantity = 0;
        }
        
        public bool CanAddItem(Item itemToAdd, int amount = 1)
        {
            if (IsEmpty) return true;
            if (item != itemToAdd) return false;
            if (!item.isStackable) return false;
            return quantity + amount <= item.maxStack;
        }
    }
    
    public List<InventorySlot> slots = new List<InventorySlot>();
    public System.Action OnInventoryChanged;
    
    [Header("Default Items (Optional)")]
  
    public Item manaPotionItem;
  
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeInventory()
    {
        slots.Clear();
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot());
        }
        
        
       
        if (manaPotionItem != null)
        {
            AddItem(manaPotionItem, 2);
        }
        else
        {
           
            AddItem("Mana Potion", 2);
        }
      
    }
    
    public bool AddItem(Item item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;
        
        
        if (item.isStackable)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].CanAddItem(item, amount))
                {
                    if (slots[i].IsEmpty)
                    {
                        slots[i].item = item;
                        slots[i].quantity = amount;
                    }
                    else
                    {
                        slots[i].quantity += amount;
                    }
                    
                    OnInventoryChanged?.Invoke();
                    Debug.Log($"Added {amount} {item.itemName} to slot {i}");
                    return true;
                }
            }
        }
        
       
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].item = item;
                slots[i].quantity = amount;
                OnInventoryChanged?.Invoke();
                Debug.Log($"Added {amount} {item.itemName} to empty slot {i}");
                return true;
            }
        }
        
        Debug.LogWarning("Inventory is full!");
        return false;
    }
    
    public bool AddItem(string itemName, int amount = 1)
    {
        
        string[] possiblePaths = {
            $"Items/{itemName}",
            itemName,
            $"Inventory/{itemName}",
            $"Resources/{itemName}"
        };
        
        foreach (string path in possiblePaths)
        {
            Item item = Resources.Load<Item>(path);
            if (item != null)
            {
                Debug.Log($"Found item '{itemName}' at path: {path}");
                return AddItem(item, amount);
            }
        }
        
       
        Item[] allItems = Resources.LoadAll<Item>("");
        foreach (Item item in allItems)
        {
            if (item.itemName == itemName || item.name == itemName)
            {
                Debug.Log($"Found item '{itemName}' by searching all items");
                return AddItem(item, amount);
            }
        }
        
        Debug.LogError($"Item not found: {itemName}");
        return false;
    }
    
    public void RemoveItem(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;
        
        InventorySlot slot = slots[slotIndex];
        if (!slot.IsEmpty)
        {
            slot.quantity -= amount;
            if (slot.quantity <= 0)
            {
                slot.Clear();
            }
            
            OnInventoryChanged?.Invoke();
            Debug.Log($"Removed {amount} from slot {slotIndex}");
        }
    }
    
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;
        
        InventorySlot slot = slots[slotIndex];
        if (!slot.IsEmpty && slot.item.isUsable)
        {
            slot.item.Use();
            RemoveItem(slotIndex, 1);
        }
    }
    
    public void SwapSlots(int slotIndexA, int slotIndexB)
    {
        if (slotIndexA < 0 || slotIndexA >= slots.Count || 
            slotIndexB < 0 || slotIndexB >= slots.Count) return;
        
        InventorySlot temp = slots[slotIndexA];
        slots[slotIndexA] = slots[slotIndexB];
        slots[slotIndexB] = temp;
        
        OnInventoryChanged?.Invoke();
    }
    
    public bool HasItem(Item item, int amount = 1)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item == item)
            {
                total += slot.quantity;
                if (total >= amount) return true;
            }
        }
        return false;
    }
    
    public int GetItemCount(Item item)
    {
        int count = 0;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item == item)
            {
                count += slot.quantity;
            }
        }
        return count;
    }
}